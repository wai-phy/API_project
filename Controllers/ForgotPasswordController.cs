using Microsoft.AspNetCore.Mvc;
using TodoApi.Repositories;
using System.ComponentModel.DataAnnotations;
using TodoApi.Models;
using TodoApi.Util;
using TodoApi.Payloads;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace TodoApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : BaseController<ForgotPasswordController>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly int _maxOTPFailCount;
        private readonly int _maxRetryOTPCount;
        private readonly int _otpExpireMinute;

        private readonly IConfiguration _configuration;

        public ForgotPasswordController(IRepositoryWrapper RW, IConfiguration configuration)
        {
            _repositoryWrapper = RW;
            _configuration = configuration;
            _maxRetryOTPCount = _configuration.GetSection("appSettings:MaxRetryOTPCount").Get<int>();
            _maxOTPFailCount = _configuration.GetSection("appSettings:OTPFailCount").Get<int>();
            _otpExpireMinute = _configuration.GetSection("appSettings:OTPExpireMinute").Get<int>();
        }

        
        [HttpPost("RequestByEmail", Name = "RequestByEmail")]
        public async Task<dynamic> RequestByEmail(ForgotPasswordEmailPayload ObjPayload)
        {
            string Email = "";
            string LoginName = "";
            try
            {
                LoginName = ObjPayload.LoginName;
                Email = ObjPayload.Email;

                if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(Email))
                {
                    var resultAdmin = await _repositoryWrapper.Admin.FindByConditionAsync(adm => adm.LoginName == LoginName); 
                    if (resultAdmin == null || !resultAdmin.Any()) 
                        throw new ValidationException("Login User " + LoginName + " not found."); 
 
                    var objAdmin = resultAdmin.First();
                    // Admin? objAdmin = await _repositoryWrapper.Admin.GetAdminByLoginName(LoginName);
                    if (objAdmin != null)
                    {
                        string ipaddress = Convert.ToString(Util.Globalfunction.GetClientIP(HttpContext));
                        int AdminID = objAdmin.AdminId;
                        if (objAdmin.Email == Email)
                        {
                            return DoOTPValidationAsync(Email, LoginName, ipaddress);
                        }
                        else
                        {
                            throw new ValidationException("Incorrect Login Name or Email.");
                        }
                    }
                    else
                    {
                        throw new ValidationException("Request Login Name not found.");
                    }
                }
                else
                {
                    throw new ValidationException("Please enter both Login Name and Email");
                }
            }
            catch (ValidationException vex)
            {
                Logger.LogWarning(vex, "Forgot Password Fail {LoginName}, {Email}", LoginName, Email);
                return new { data = 0, error = vex.ValidationResult.ErrorMessage };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Forgot Password Fail {LoginName}, {Email}", LoginName, Email);
                return new { data = 0, error = "Forgot Password Fail" };
            }
        }

        private dynamic DoOTPValidationAsync(string Email, string LoginName, string ipaddress)
        {
            string[] OTPAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string[] PrefixAllowedCharacters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                
            var otpObject = _repositoryWrapper.OTP.FindByCondition(x => x.EmailPhone == Email && x.LoginName == LoginName).SingleOrDefault();
            if (otpObject == null)
            {
                otpObject = new()
                {
                    EmailPhone = Email,
                    LoginName = LoginName,
                    OTPToken = "otptoken",
                    SendDateTime = DateTime.Now,
                    FailCount = 0,
                    RetryCount = 0,
                    CreatedDate = DateTime.Now
                };
            }
            else {
                DateTime value_plus_hrs = otpObject.SendDateTime.AddHours(24);
                if (DateTime.Now > value_plus_hrs)
                {
                    otpObject.RetryCount = 0;
                    otpObject.FailCount = 0;
                }
            }
            // string passcode = otpObject.passcode;
            // string prefix_char = passcode.Substring(0, 1);

            string RandomCharString = Globalfunction.GenerateRandomOTP(6, OTPAllowedCharacters);
            string sRandomChar = Globalfunction.GenerateRandomChar(PrefixAllowedCharacters);
            string sRandomOTP = sRandomChar + "-" + RandomCharString;
            
            if (otpObject.RetryCount < _maxRetryOTPCount)
            {
                otpObject.Passcode = sRandomOTP;
                otpObject.SendDateTime = DateTime.Now;
                otpObject.IPAddress = ipaddress;
                otpObject.FailCount = 0;
                otpObject.RetryCount = otpObject.RetryCount + 1;
                otpObject.LastModifiedDate = DateTime.Now;
                if(otpObject.OTPId > 0) {
                    _repositoryWrapper.OTP.Update(otpObject);
                }
                else {
                    _repositoryWrapper.OTP.Create(otpObject);
                }
                
                string messagebody = @"
                Dear " + LoginName + "\r\n" +
                "Your OTP code to reset your password is " + sRandomOTP;

                Util.Globalfunction.SendEmailAsync(Email, "Forgot Password", messagebody, false);
                //Console.WriteLine(messagebody);
                return new { Message = "Successful generate OTP code", data = 1, prefix_char = sRandomChar, sendername = "Email" };
            }
            else
            {
                throw new ValidationException("You have been reached max OTP code, please try again after 24 hrs");
            }
        }

        [HttpPost("ChangePasswordByOTP", Name = "ChangePasswordByOTP")]
        public async Task<dynamic> ChangePasswordByOTP(ChangePasswordOTPPayload ObjPayload)
        {
            string EP_value = ObjPayload.Email;
            string otpcode = ObjPayload.OTPPrefix + "-" + ObjPayload.OTPPasscode;
            string new_password = ObjPayload.Password;
            string confirm_password = ObjPayload.ConfirmPassword;
            string LoginName = ObjPayload.LoginName;

            try
            {
                if(new_password != confirm_password)
                    throw new ValidationException("Password and confirm password not match.");

                Globalfunction.CheckPassword(new_password);  //it will throw validation exception if invalid password.
                

                var objAdmin = (await _repositoryWrapper.Admin.FindByConditionAsync(x => x.LoginName == LoginName)).SingleOrDefault();
                if (objAdmin != null)
                {
                    
                    var otpObject = (await _repositoryWrapper.OTP.FindByConditionAsync(x => x.EmailPhone == EP_value && x.LoginName == LoginName)).SingleOrDefault();
                    if (otpObject != null)
                    {
                        if(otpObject.SendDateTime.AddMinutes(_otpExpireMinute) < DateTime.Now)
                        {
                            throw new ValidationException("OTP code is expired, please request OTP again.");
                        }

                        string passcode = otpObject.Passcode;
                        // Check otp code
                        if(otpObject.FailCount >= _maxOTPFailCount) {
                            throw new ValidationException("You have been reached a lot of wrong OTP code, please request OTP again");
                        }

                        if (otpcode == passcode)
                        {
                            otpObject.FailCount = 0;
                            otpObject.RetryCount = 0;
                            otpObject.LastModifiedDate = DateTime.Now;
                            await _repositoryWrapper.OTP.UpdateAsync(otpObject);

                            string salt = "";
                            string password = "";
                            salt = Util.SaltedHash.GenerateSalt();
                            password = Util.SaltedHash.ComputeHash(salt, new_password);

                            objAdmin.Salt = salt;
                            objAdmin.Password = password;
                            // objAdmin.ModifiedDate = DateTime.Now;
                            await _repositoryWrapper.Admin.UpdateAsync(objAdmin);
                            await _repositoryWrapper.EventLog.Info("Reset Password Successful: " + objAdmin.LoginName);

                            return new { data = 1, Message = "Successfully change password" };
                        }
                        else
                        {
                            otpObject.FailCount = otpObject.FailCount + 1;
                            otpObject.LastModifiedDate = DateTime.Now;
                            await _repositoryWrapper.OTP.UpdateAsync(otpObject);
                            throw new ValidationException("Your OTP code is wrong, please try again.");
                        }
                    }
                    else
                    {
                        throw new ValidationException("Invalid Login Name or Email");
                    }
                }
                else
                {
                    throw new ValidationException("Invalid Login Name");
                }
            }
            catch (ValidationException vex)
            {
                Logger.LogWarning(vex, "ChangePasswordByOTP Fail: {LoginName}, {Email}", LoginName, EP_value);
                await _repositoryWrapper.EventLog.Warning("Change Password Fail: " + vex.Message);
                return new { data = 0, error = vex.ValidationResult.ErrorMessage };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ChangePasswordByOTP Fail: {LoginName}, {Email}", LoginName, EP_value);
                await _repositoryWrapper.EventLog.Error("Change Password Fail", ex.Message);
                return new { data = 0, error = "ChangePasswordByOTP Fail" };
            }
        }
    }
    
}