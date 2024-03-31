using ibanking_server.Data;
using ibanking_server.Enums;
using ibanking_server.Exceptions;
using ibanking_server.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace ibanking_server.Utils
{
    public class OTPUtils
    {
        private readonly BankingDbContext dbContext;

        public OTPUtils(BankingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string GenerateOTP()
        {
            //string otp = "";
            //bool checkOTP = true;

            //do
            //{
            //    otp = RandomOTP();
            //    checkOTP = await IsExistedOTP(otp, accountId);
            //} while (checkOTP);

            //return otp;
            Random random = new Random();
            string result = "";

            for (int i = 0; i < 6; i++)
            {
                result += random.Next(0, 10).ToString();
            }

            return result;
        }

        //public async Task<bool> IsExistedOTP(string otp, int accountId)
        //{
        //    OTP? existedOTP = await dbContext.OTPs
        //        .SingleOrDefaultAsync(o => o.OTPCode.Equals(otp) && o.AccountId == accountId);
        //    return existedOTP != null;
        //}

        //private string RandomOTP()
        //{
        //    Random random = new Random();
        //    string result = "";

        //    for (int i = 0; i < 6; i++)
        //    {
        //        result += random.Next(0, 10).ToString();
        //    }

        //    return result;
        //}

        public async Task<bool> VerifyOTP(string otp, int transactionId)
        {
            OTP existedOTP = await dbContext.OTPs
                .SingleOrDefaultAsync(o => o.OTPCode.Equals(otp) && o.TransactionId == transactionId) ??
                    throw new InvalidOTPException("OTP is invalid");

            if (existedOTP.ExpiredAt < DateTime.Now)
            {
                throw new InvalidOTPException("OTP is expired");
            }

            existedOTP.OTPStatus = OTPStatus.EXPIRED;
            await dbContext.SaveChangesAsync();
            return true;
        }

    }
}
