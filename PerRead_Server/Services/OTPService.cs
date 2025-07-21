using Microsoft.Extensions.Caching.Memory;
using System;

namespace PerRead_Server.Services
{
    public class OTPService
    {
        private readonly IMemoryCache _memoryCache;

        public OTPService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void StoreOtp(string email, string otp)
        {
            var cacheKey = $"OTP_{email}";
            var expirationTime = TimeSpan.FromMinutes(5);

            _memoryCache.Set(cacheKey, otp, expirationTime);
        }

        public string? GetOtp(string email)
        {
            var cacheKey = $"OTP_{email}";
            return _memoryCache.Get<string>(cacheKey);
        }

        public bool ValidateOtp(string email, string otp)
        {
            var storedOtp = GetOtp(email);
            return storedOtp == otp;
        }

        public void RemoveOtp(string email)
        {
            var cacheKey = $"OTP_{email}";
            _memoryCache.Remove(cacheKey);
        }
    }
}
