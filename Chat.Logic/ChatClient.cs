using Contracts;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Chat.Logic
{
    public static class ChatClient
    {
        #region Constants

        const int _connectionTimeoutSeconds = 100;

        const int _checkConnectionMilliseconds = 2000;

        #endregion Constants

        #region Proxy Implementation

        static DuplexChannelFactory<IChatService> factory;
        static IChatService proxy;

        private static void OpenChannel()
        {
            try
            {
                factory = new DuplexChannelFactory<IChatService>(new ChatResult(), "DuplexEP");
                proxy = factory.CreateChannel();
                //Define an operation timeout to limit the operation time
                ((IClientChannel)proxy).OperationTimeout = TimeSpan.FromSeconds(_connectionTimeoutSeconds);
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
        }


        #endregion Proxy Implementation

        #region IChatService Client Implementation

        public static string Login(string userName, string password)
        {
            OpenChannel();
            Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel opened for client {userName}");
            bool isFault = false;
            try
            {
                LoginError error = proxy.Login(userName, password);
                if (error == LoginError.NoError)
                {
                    StartTimer();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Client {userName} connected");
                }
                else
                {
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Client {userName} was not connected");
                }

                //Check connection to server every _checkConnectionSeconds 

                return GenerateErrorString(error);
            }
            catch (EndpointNotFoundException)
            {
                isFault = true;
                string message = "Connection error - Unable to connect to server";
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, message);
                throw new EndpointNotFoundException(message);
            }
            catch (Exception e)
            {
                isFault = true;
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
            finally
            {
                if (isFault)
                {
                    factory.Abort();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel closed for client {userName} after exception");
                    OpenChannel();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel opened for client {userName} after exception");
                }
            }
        }

        public static bool Register(string userName, string password)
        {
            OpenChannel();
            bool isFault = false;
            try
            {
                if (proxy.Register(userName, password))
                {
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Client {userName} registered");
                    StartTimer();
                    return true;
                }
                else
                {
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Client {userName} was not registered");

                    return false;
                }
            }
            catch (EndpointNotFoundException)
            {
                isFault = true;
                string message = "Connection error - Unable to connect to server";
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, message);
                throw new EndpointNotFoundException(message);
            }
            catch (Exception e)
            {
                isFault = true;
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
            finally
            {
                if (isFault)
                {
                    factory.Abort();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel closed for client {userName} after exception");
                    OpenChannel();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel opened for client {userName} after exception");
                }
            }
        }

        public static Guid? AddSession(string sender, string reciever)
        {
            Guid? session;
            try
            {
                session = proxy.Chat(sender, reciever);
                if (session != null)
                {
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Session {session} opened with user {reciever}");
                    return session;
                }
                else
                {
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Warning, $"Session with user {reciever} was not opened");
                    return null;
                }
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
        }

        public static void logoff(string userName)
        {
            bool isFault = false;
            try
            {
                proxy.LogOut(userName);
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Client {userName} logged out");
                factory.Close();
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel for Client {userName} closed");
                StopConnectionTimer();
            }
            catch (Exception e)
            {
                isFault = true;
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
            finally
            {
                if (isFault)
                {
                    factory.Abort();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel closed for client {userName} after exception");
                    OpenChannel();
                    Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Channel opened for client {userName} after exception");
                }
            }
        }

        public static void CloseChat(Guid session)
        {
            proxy.EndChat(session);
        }

        public static bool SendMessage(Guid session, string message)
        {
            try
            {
                bool success = proxy.SendMessage(session, message);
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"Message sent in session {session}");
                return success;
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, $"Message was nit send in session {session}");
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                throw e;
            }
        }

        #endregion IChatService Client Implementation

        #region IBackgammon Client Implementation

        public static Guid? RequestGame(string userName, string contact)
        {
            try
            {
                Guid? gameId = proxy.RequestNewGame(userName, contact);
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Information, $"New game opened {gameId}");
                return gameId;
            }
            catch (Exception e)
            {
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, $"Game was not opened");
                Report.log(DeviceToReport.Client_Proxy, LogLevel.Exception, e.Message);
                return null;
            }
        }

        #endregion IBackgammon Client Implementation

        #region Private Methods

        private static string GenerateErrorString(LoginError error)
        {
            switch (error)
            {
                case LoginError.NoError:
                    return "";
                case LoginError.UsernameDoesNotExist:
                    return "Username does not exist";
                case LoginError.WrongPassword:
                    return "Wrong Password";
                case LoginError.UserAlreadyConnected:
                    return "User is already connected to server";
                case LoginError.UnknownException:
                    return "Unknown exception in the server";
                default:
                    return "";
            }
        }

        #endregion Private Methods

        #region Timed Check Connection

        public static event EventHandler ConnectionLostEvent;

        private static Timer _timer;

        private static void CheckConnection(object sender, EventArgs e)
        {
            try
            {
                proxy.IsActive();
            }
            catch (Exception)
            {
                ConnectionLostEvent?.Invoke(null, null);
                StopConnectionTimer();
            }

        }

        private static void StartTimer()
        {
            _timer = new Timer();
            _timer.Interval = _checkConnectionMilliseconds;
            _timer.Elapsed += CheckConnection;
            _timer.Enabled = true;
        }

        public static void StopConnectionTimer()
        {
            _timer.Enabled = false;
        }

        #endregion Timed Check Connection
    }
}
