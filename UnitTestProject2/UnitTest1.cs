using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chat.Client.VM;
using Chat.UI.Views.Screens;
using Chat.UI.VM;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MainVM vm = MainVM.Instance;
            vm.UserName = "a";

            ChatWindow cw = new ChatWindow("b", true);

        }
    }
}
