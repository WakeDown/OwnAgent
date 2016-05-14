using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public partial class AspNetUsers
    {
        public HashSet<UserGroup> AdGroups { get; private set; } = new HashSet<UserGroup>();
        public void SetUserGroups(IEnumerable<UserGroup> groups) => AdGroups = new HashSet<UserGroup>(groups);
        
        public static string ShortName(string fullName)
        {
            if (String.IsNullOrEmpty(fullName)) return String.Empty;
            string result = String.Empty;
            string[] nameArr = fullName.Split(' ');
            for (int i = 0; i < nameArr.Count(); i++)
            {
                //if (i > 2) break;
                string name = nameArr[i];
                if (String.IsNullOrEmpty(name)) continue;
                if (i > 0) name = name[0] + ".";
                if (i == 1) name = " " + name;
                result += name;
            }
            return result;
        }

        public bool Is(params UserGroup[] groups) => groups.Any(grp => AdGroups.Contains(grp));

        public bool HasAccess(params UserGroup[] groups)
        {
            if (AdGroups == null || !AdGroups.Any()) return false;
            //if (AdGroups.Contains(UserGroup.SuperAdmin) || AdGroups.Contains(UserGroup.TaskTrackerAdmin)) return true;
            return groups.Any(grp => AdGroups.Contains(grp));
        }
    }
}
