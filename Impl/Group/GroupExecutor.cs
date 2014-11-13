using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OsnTester.OsnProxy;

namespace OsnTester.Impl.Group
{
    /// <summary>
    /// OsnGroup
    /// </summary>
    public class GroupExecutor : OsnExecutor
    {
        public GroupExecutor()
            : base(new GroupInterpreter())
        {
        }
        
        // Public instance properties


        // Public instance methods
        public override bool CheckInput()
        {
            return true;
        }

        public override void Prompt()
        {
            Output(@"Usage: group command 
       group create name
       group delete|get id
       group rename id newName
       group list

group is a set of osn server.

Commands:
    create - Create a new group
    delete - Delete groups
    rename - Rename name of the groups");
        }

        private string Create()
        {
            if (paras.Count < 1)
            {
                return "fatal error: missing parameters";
            }

            string groupName = paras[0];

            OSNStatus status = platformClient.ProxyCreateServerGroup(groupName);

            string msg = String.Format("create group {0} {1}.", groupName, interpreter.TranslateCommon(status.RetCode));
            return msg;
        }

        private string Delete()
        {
            if (paras.Count < 1)
            {
                return "fatal error: missing parameters";
            }

            string groupId = paras[0];

            OSNStatus status = platformClient.ProxyDeleteServerGroup(groupId);
            string msg = String.Format("delete group {0} {1}.", groupId, interpreter.TranslateCommon(status.RetCode));
            return msg;
        }

        private string Rename()
        {
            if (paras.Count < 2)
            {
                return "fatal error: missing parameters";
            }

            string groupId = paras[0];
            string newName = paras[1];

            OSNStatus status = platformClient.ProxyRenameServerGroup(groupId, newName);
            string msg = String.Format("rename group {0} {1}.", groupId, interpreter.TranslateCommon(status.RetCode));
            return msg;
        }
    }
}
