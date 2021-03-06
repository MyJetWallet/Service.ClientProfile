using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyJetWallet.ServiceBus.SessionAudit.Models;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Grpc.Models.Requests.Blockers;
using Service.ClientProfile.Grpc.Models.Responses;
using Service.ClientProfile.Postgres;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;

namespace Service.ClientProfile.Services
{
    public class ClientProfileService
    {
        private readonly ILogger<ClientProfileServiceGrpc> _logger;
        private readonly IServiceBusPublisher<ClientProfileUpdateMessage> _publisher;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ProfileCacheManager _cache;
        private readonly IPersonalDataServiceGrpc _personalDataService;

        public ClientProfileService(IServiceBusPublisher<ClientProfileUpdateMessage> publisher,
            ILogger<ClientProfileServiceGrpc> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ProfileCacheManager cache, IPersonalDataServiceGrpc personalDataService)
        {
            _publisher = publisher;
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _cache = cache;
            _personalDataService = personalDataService;
        }

        public async IAsyncEnumerable<BlockerGrpcModel> GetClientProfileBlockers(
            GetClientProfileBlockersRequest request)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var clientBlockers = context.Blockers
                .Include(t => t.Profile)
                .Where(itm => itm.ExpiryTime > DateTime.UtcNow);

            if (request.Type != null)
            {
                clientBlockers = clientBlockers.Where(itm => itm.BlockedOperationType == request.Type);
            }

            if (!string.IsNullOrEmpty(request.ClientId))
            {
                clientBlockers = clientBlockers.Where(itm => itm.Profile.ClientId == request.ClientId);
            }

            await foreach (var blocker in clientBlockers.AsAsyncEnumerable())
            {
                yield return new BlockerGrpcModel
                {
                    BlockerId = blocker.BlockerId,
                    BlockedOperationType = blocker.BlockedOperationType,
                    ExpiryTime = blocker.ExpiryTime,
                    Reason = blocker.Reason,
                    LastTs = blocker.LastTs,
                    ClientId = blocker?.Profile?.ClientId,
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> AddBlockerToClient(AddBlockerToClientRequest request)
        {
            _logger.LogInformation(
                "Adding blocker for clientId {clientId}, type {type}, reason {reason}, expiry time {expiryTime}",
                request.ClientId, request.Type.ToString(), request.BlockerReason, request.ExpiryTime);
            try
            {
                if (request.ExpiryTime < DateTime.UtcNow)
                    return new ClientProfileUpdateResponse
                    {
                        IsSuccess = false,
                        ClientId = request.ClientId,
                        Error = "Blocker expiry time already passed"
                    };

                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var dbProfile = context.ClientProfiles.FirstOrDefault(itm => itm.ClientId == request.ClientId);
                if (dbProfile == null)
                {
                    return new ClientProfileUpdateResponse
                    {
                        IsSuccess = false,
                        ClientId = request.ClientId,
                        Error = "Client not found in db"
                    };
                }

                dbProfile.Blockers ??= new List<Blocker>();

                dbProfile.Blockers.Add(new Blocker
                {
                    Reason = request.BlockerReason,
                    BlockedOperationType = request.Type,
                    ExpiryTime = request.ExpiryTime,
                    Profile = profile
                });
                await context.SaveChangesAsync();

                await _publisher.PublishAsync(new ClientProfileUpdateMessage
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });


                var clientBlockers =
                    context.Blockers.Where(itm => itm.Profile.ClientId == request.ClientId).ToList();

                var profileAfterSave = await context.ClientProfiles
                    .FirstOrDefaultAsync(itm => itm.ClientId == request.ClientId);

                if (profileAfterSave == null)
                    return new ClientProfileUpdateResponse
                    {
                        IsSuccess = true,
                        ClientId = request.ClientId
                    };


                profileAfterSave.Blockers = clientBlockers;
                await _cache.AddOrUpdateClientProfile(profileAfterSave);

                return new ClientProfileUpdateResponse
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When adding blocker to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> DeleteBlockerFromClient(DeleteBlockerFromClientRequest request)
        {
            _logger.LogInformation("Removing blocker for clientId {clientId}, blockerId {blockerId}", request.ClientId,
                request.BlockerId.ToString());
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                var blocker = profile.Blockers?.FirstOrDefault(t => t.BlockerId == request.BlockerId);
                if (blocker == null)
                    return new ClientProfileUpdateResponse()
                    {
                        ClientId = request.ClientId,
                        IsSuccess = false,
                        Error = "Blocker not found"
                    };

                profile.Blockers.Remove(blocker);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                context.Blockers.Remove(blocker);
                context.ClientProfiles.Update(profile);
                await context.SaveChangesAsync();
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When removing blocker to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> Enable2Fa(Enable2FaRequest request)
        {
            _logger.LogInformation("Enabling 2FA for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.Status2FA = Status2FA.Enabled;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When enabling 2FA to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> Disable2Fa(Disable2FaRequest request)
        {
            _logger.LogInformation("Disabling 2FA for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.Status2FA = Status2FA.Disabled;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When disabling 2FA to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<Domain.Models.ClientProfile> GetOrCreateProfile(GetClientProfileRequest request) =>
            await GetOrCreateProfile(request.ClientId);

        public async Task<GetAllClientProfilesResponse> GetAllProfiles()
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entities = context.ClientProfiles
                .Include(t => t.Blockers)
                .ToList();

            return new GetAllClientProfilesResponse()
            {
                ClientProfiles = entities
            };
        }

        public async Task<GetAllClientProfilesResponse> GetAllProfiles(GetAllRequest request)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entities = context.ClientProfiles
                .Skip(request.Skip)
                .Take(request.Take)
                .Include(t => t.Blockers)
                .ToList();

            return new GetAllClientProfilesResponse()
            {
                ClientProfiles = entities
            };
        }


        private async Task<Domain.Models.ClientProfile> GetOrCreateProfile(string clientId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var clientProfile = context.ClientProfiles
                .Include(t => t.Blockers)
                .FirstOrDefault(t => t.ClientId == clientId);

            if (clientProfile != null)
            {
                if (string.IsNullOrEmpty(clientProfile.ReferralCode))
                {
                    clientProfile.ReferralCode = await AddReferralCodeToExistingUser(clientProfile);
                }

                if (string.IsNullOrEmpty(clientProfile.ExternalClientId))
                {
                    clientProfile.ExternalClientId = await AddExternalClientIdToExistingUser(clientProfile);
                }

                return clientProfile;
            }

            _logger.LogInformation("Profile for clientId {clientId} not found. Creating new profile", clientId);

            var profile = new Domain.Models.ClientProfile
            {
                ClientId = clientId,
                Status2FA = Status2FA.NotSet,
                Blockers = new List<Blocker>(),
                EmailConfirmed = false,
                PhoneConfirmed = false,
                KYCPassed = false,
                ReferralCode = await GenerateReferralCode(context, clientId),
                ExternalClientId = await GenerateExternalClientId(clientId),
                MarketingEmailAllowed = true
            };

            await context.UpsertAsync(profile);
            await _cache.AddOrUpdateClientProfile(profile);

            return profile;
        }

        private async Task<string> GenerateExternalClientId(string clientId)
        {
            if (String.IsNullOrEmpty(clientId))
                return String.Empty;

            var pd = await _personalDataService.GetByIdAsync(new GetByIdRequest()
            {
                Id = clientId
            });

            if (pd.PersonalData == null)
                return String.Empty;

            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(pd.PersonalData.Email);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
        }

        private async Task<string> AddExternalClientIdToExistingUser(Domain.Models.ClientProfile profile)
        {
            _logger.LogInformation("Generating external client id for {clientId}", profile.ClientId);

            var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var externalClientId = await GenerateExternalClientId(profile.ClientId);
            profile.ExternalClientId = externalClientId;

            await context.UpsertAsync(profile);
            await _cache.AddOrUpdateClientProfile(profile);

            await _publisher.PublishAsync(new ClientProfileUpdateMessage()
            {
                OldProfile = oldProfile,
                NewProfile = profile
            });

            return externalClientId;
        }

        private async Task<string> AddReferralCodeToExistingUser(Domain.Models.ClientProfile profile)
        {
            _logger.LogInformation("Generating Referral code for clientId {clientId}", profile.ClientId);

            var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var referralCode = await GenerateReferralCode(context, profile.ClientId);
            profile.ReferralCode = referralCode;

            await context.UpsertAsync(profile);
            await _cache.AddOrUpdateClientProfile(profile);

            await _publisher.PublishAsync(new ClientProfileUpdateMessage()
            {
                OldProfile = oldProfile,
                NewProfile = profile
            });

            return referralCode;
        }

        private async Task<string> GenerateReferralCode(DatabaseContext ctx, string clientId)
        {
            var codes = new List<string>();

            var str = clientId.Replace("-", "").ToUpper();
            for (int i = 0; i < str.Length - 6; i++)
            {
                codes.Add(str.Substring(i, 6));
            }

            var existsClients = await ctx.ClientProfiles
                .Where(e => codes.Contains(e.ReferralCode))
                .Select(e => e.ReferralCode)
                .ToListAsync();

            codes = codes.Where(e => !existsClients.Contains(e)).ToList();

            if (codes.Any())
                return codes.First();

            var countIterations = 0;
            while (countIterations < 10)
            {
                for (int k = 0; k < 3; k++)
                {
                    str = Guid.NewGuid().ToString("N").Replace("-", "").ToUpper();
                    for (int i = 0; i < str.Length - 6; i++)
                    {
                        codes.Add(str.Substring(i, 6));
                    }
                }

                existsClients = await ctx.ClientProfiles
                    .Where(e => codes.Contains(e.ReferralCode))
                    .Select(e => e.ReferralCode)
                    .ToListAsync();

                codes = codes.Where(e => !existsClients.Contains(e)).ToList();

                if (codes.Any())
                    return codes.First();

                countIterations++;
            }

            throw new Exception($"Cannot generate Referrer Code for client {clientId}");
        }

        public async Task<ClientProfileUpdateResponse> UpdateConfirmStatuses(string clientId, bool phoneConfirmed,
            bool emailConfirmed)
        {
            _logger.LogInformation("Updating confirm statuses for clientId {clientId}", clientId);
            try
            {
                var profile = await GetOrCreateProfile(clientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.EmailConfirmed = emailConfirmed;
                profile.PhoneConfirmed = phoneConfirmed;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = clientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When disabling 2FA to client {clientId}", clientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = clientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> SetKYCPassed(SetKYCPassedRequest request)
        {
            _logger.LogInformation("Setting KYC for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.KYCPassed = true;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting KYC to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientByReferralResponse> GetProfileByReferralCode(string code)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var profile = context.ClientProfiles.FirstOrDefault(t => t.ReferralCode == code);

            return profile != null
                ? new ClientByReferralResponse() {IsExists = true, ClientId = profile.ClientId}
                : new ClientByReferralResponse() {IsExists = false};
        }

        public async Task<GetAllClientProfilesResponse> GetReferrals(string clientId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entities = context.ClientProfiles
                .Where(t => t.ReferrerClientId == clientId)
                .Include(t => t.Blockers)
                .ToList();

            return new GetAllClientProfilesResponse()
            {
                ClientProfiles = entities
            };
        }

        public async Task<ClientProfileUpdateResponse> AddReferral(AddReferralRequest request)
        {
            _logger.LogInformation("Adding referral for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                var referrer = await GetProfileByReferralCode(request.ReferralCode);
                if (!referrer.IsExists)
                {
                    return new ClientProfileUpdateResponse()
                    {
                        IsSuccess = false,
                        ClientId = request.ClientId,
                        Error = "Invalid referral code. User with this code doesn't exist"
                    };
                }

                profile.ReferrerClientId = referrer.ClientId;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When adding referral to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> ChangeReferralCode(ChangeReferralCodeRequest request)
        {
            _logger.LogInformation("Changing referral code for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                if (string.IsNullOrWhiteSpace(request.ReferralCode))
                {
                    return new ClientProfileUpdateResponse()
                    {
                        IsSuccess = false,
                        ClientId = request.ClientId,
                        Error = "Invalid referral code."
                    };
                }

                var referrer = await GetProfileByReferralCode(request.ReferralCode);
                if (referrer.IsExists)
                {
                    return new ClientProfileUpdateResponse()
                    {
                        IsSuccess = false,
                        ClientId = request.ClientId,
                        Error = "Invalid referral code. User with this code already exists"
                    };
                }

                profile.ReferralCode = request.ReferralCode;


                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When changing referral code to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<GetAllClientProfilesResponse> GetProfileByExternalId(string externalId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var list = context.ClientProfiles
                .Where(t => t.ExternalClientId.Contains(externalId.ToLower()) || t.ExternalClientId.Contains(externalId.ToUpper())).ToList();
            return new GetAllClientProfilesResponse()
            {
                ClientProfiles = list
            };
        }

        public async Task<ClientProfileUpdateResponse> SetMarketingEmailSettings(
            SetMarketingEmailSettingsRequest request)
        {
            _logger.LogInformation("Setting marketing email settings for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.MarketingEmailAllowed = request.IsAllowed;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting marketing email to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> SetUserType(SetUserTypeRequest request)
        {
            _logger.LogInformation("Setting user type for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.UserType = request.UserType;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting user type to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }

        public async Task<ClientProfileUpdateResponse> SetInternalSimpleEmailAsync(SetInternalSimpleEmailRequest request)
        {
            _logger.LogInformation("Setting InternalSimpleEmail for clientId {clientId}", request.ClientId);
            try
            {
                var profile = await GetOrCreateProfile(request.ClientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.InternalSimpleEmail = request.InternalSimpleEmail;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting InternalSimpleEmail to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }
        
        public async Task SetSubmitReviewFlag(string clientId, bool shouldAsk)
        {
            _logger.LogInformation("Setting InternalSimpleEmail for clientId {clientId}", clientId);
            try
            {
                var profile = await GetOrCreateProfile(clientId);
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                if(profile.AskToSubmitReview == shouldAsk)
                    return;
                
                profile.AskToSubmitReview = shouldAsk;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);
                
                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting InternalSimpleEmail to client {clientId}", clientId);
            }
        }
        
        public async Task<ClientProfileUpdateResponse> SubmitReview(SubmitReviewRequest request)
        {
            _logger.LogInformation("Submitting review for clientId {clientId}", request.ClientId);
            try
            {
                var review = new ReviewResult
                {
                    ClientId = request.ClientId,
                    Timestamp = DateTime.UtcNow,
                    Stars = request.Stars,
                    Action = request.Action,
                    ReviewText = request.ReviewText
                };
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                context.ReviewResults.Add(review);
                await context.SaveChangesAsync();

                await CheckIsShouldAskAndUpdateProfile(request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = request.ClientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When submitting review to client {clientId}", request.ClientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = request.ClientId,
                    Error = e.Message
                };
            }
        }
        
        public async Task<bool> CheckIsShouldAskAndUpdateProfile(string clientId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var hasReview = await context.ReviewResults.Where(t => t.ClientId == clientId).AnyAsync();

            await SetSubmitReviewFlag(clientId, !hasReview);
            return !hasReview;
        }
        
        public async Task<ClientProfileUpdateResponse> SetUserAgentDataAsync(
            string clientId, DeviceOperationSystem deviceOperationSystem, bool isMobile)
        {
            _logger.LogInformation("Setting SetUserAgentData for clientId {clientId}", clientId);
            try
            {
                var profile = await GetOrCreateProfile(clientId);
                
                if (profile.IsMobile == isMobile && profile.DeviceOperationSystem == deviceOperationSystem)
                    return new ClientProfileUpdateResponse()
                    {
                        IsSuccess = true,
                        ClientId = clientId
                    };
                
                var oldProfile = (Domain.Models.ClientProfile) profile.Clone();

                profile.DeviceOperationSystem = deviceOperationSystem;
                profile.IsMobile = isMobile;

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.UpsertAsync(profile);
                await _cache.AddOrUpdateClientProfile(profile);

                await _publisher.PublishAsync(new ClientProfileUpdateMessage()
                {
                    OldProfile = oldProfile,
                    NewProfile = profile
                });

                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = true,
                    ClientId = clientId
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When setting SetUserAgentData to client {clientId}", clientId);
                return new ClientProfileUpdateResponse()
                {
                    IsSuccess = false,
                    ClientId = clientId,
                    Error = e.Message
                };
            }
        }
    }
}