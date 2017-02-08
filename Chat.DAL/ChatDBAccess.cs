using Contracts;
using Chat.DAL;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DAL
{
    public static class ChatDBAccess
    {
        #region Chat Handling

        #region Users Table

        public static bool AddUser(string userName, string password)
        //Adds user to DB. If user exists return False
        {
            try
            {
                using (var ctx = new ChatContext())
                {
                    //userName is not in DB
                    if (!IsUserInDB(userName))
                    {
                        User user = new User() { UserName = userName };
                        user.Password = Encryption.Encrypt(password);
                        ctx.Users.Add(user);
                        ctx.SaveChanges();
                        Report.log(DeviceToReport.DAL, LogLevel.Information, $"User {userName} Created in DB");
                        return true;
                    }

                    //userName exists in DB
                    Report.log(DeviceToReport.DAL, LogLevel.Information, $"User {userName} already exists in DB");

                    return false;
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Exception, $"User {userName} was not added to DB");
                Report.log(DeviceToReport.DAL, LogLevel.Exception, e.Message);
                throw e;
            }
        }

        public static bool IsUserInDB(string userName)
        //Returns True if username is taken, False otherwise
        {
            try
            {
                using (var ctx = new ChatContext())
                {
                    //userName is not in DB
                    if (ctx.Users.FirstOrDefault(u => u.UserName.ToUpper() == userName.ToUpper()) == null)
                    {
                        return false;
                    }

                    //userName exists in DB
                    return true;
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Exception, $"Error while searching DB for {userName}");
                Report.log(DeviceToReport.DAL, LogLevel.Exception, e.Message);
                throw e;
            }
        }

        public static bool IsPasswordCorrect(string userName, string password)
        {
            try
            {
                using (var ctx = new ChatContext())
                {
                    User user = ctx.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        string Password = Encryption.Decrypt(user.Password);
                        if (Password == password)
                        {
                            Report.log(DeviceToReport.DAL, LogLevel.Information, $"Password for user {userName} is correct");
                            return true;
                        }
                        else
                        {
                            Report.log(DeviceToReport.DAL, LogLevel.Information, $"Password for user {userName} is not correct");
                            return false;
                        }
                    }
                    Report.log(DeviceToReport.DAL, LogLevel.Exception, $"User {userName} was not found in DB");
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Exception, $"Error while searching DB for {userName}");
                Report.log(DeviceToReport.DAL, LogLevel.Exception,e.Message);

                throw e;
            }
        }

        public static List<string> GetAllUsers()
        {
            List<string> users = new List<string>();
            try
            {
                using (var ctx = new ChatContext())
                {
                    foreach (User u in ctx.Users)
                    {
                        users.Add(u.UserName);
                    }

                }
                return users;
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Information, "Could not get list of users from DB");
                Report.log(DeviceToReport.DAL, LogLevel.Information, e.Message);
                throw e;
            }
        }

        #endregion Users Table

        #region Messages Table

        public static void AddMessage(Message message)
        //Adds message from user to contact to the DB
        {
            try
            {
                using (var ctx = new ChatContext())
                {
                    ctx.Messages.Add(message);
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Information, $"Message from {message.Sender} to {message.Receiver} was not added to DB");
                Report.log(DeviceToReport.DAL, LogLevel.Information, e.Message);
                throw e;
            }
        }

        public static List<Message> GetListOfMessages(string userA, string userB)
        {
            List<Message> messages = new List<Message>();
            try
            {
                using (var ctx = new ChatContext())
                {
                    messages = ctx.Messages.Where(m => (m.Sender == userA ||m.Receiver == userA) &&
                                                       (m.Sender == userB || m.Receiver == userB))
                        .ToList();
                    return messages.OrderBy(m => m.SentTime).ToList();
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.DAL, LogLevel.Information, $"could not retrieve message between {userA} and {userB}");
                Report.log(DeviceToReport.DAL, LogLevel.Information, e.Message);
                throw e;
            }
        }

        #endregion Messages Table

        #endregion Chat Handling
    }
}
