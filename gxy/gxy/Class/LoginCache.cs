using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace gxy
{
    class LoginCache
    {
        static LoginCache logincache = null;
        private LoginCache(){}
        public static LoginCache Get()
        {
            if (logincache == null) logincache = new LoginCache();
            return logincache;
        }
        string path = Directory.GetCurrentDirectory() + @"\UserData_LoginCache.txt";
        JArray User_List_Array = new JArray();

        public void Init()
        {
            if (File.Exists(path))
            {
                try 
                {
                    User_List_Array = JArray.Parse(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Form1.f1.Log("读取用户登录信息文件时出现错误 原因:" + ex.Message);
                }
            }
        }
        public void Write()
        {
            try
            {
                File.WriteAllText(path, User_List_Array.ToString());
            }
            catch(Exception ex)
            {
                Form1.f1.Log("保存用户登录信息文件时出现错误 原因:" + ex.Message);
            }
        }
        public bool isExist(string account)
        {
            foreach (JToken jtoken in User_List_Array)
            {
                if (jtoken["account"].ToString().Equals(account))
                {
                    return true;
                }
            }
            return false;
        }
        public int getIndex(string account)
        {
            for (int i = 0; i < User_List_Array.Count; i++)
            {
                if (User_List_Array[i]["account"].ToString().Equals(account))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Insert(string account, string userid, string token, string planid)
        {
            try
            {
                JObject jobject = new JObject();
                jobject["account"] = account;
                jobject["userid"] = userid;
                jobject["token"] = token;
                jobject["planid"] = planid;
                jobject["UpdateTime"] = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
                User_List_Array.Add(jobject);
                Write();
                return true;
            }
            catch(Exception ex)
            {
                Form1.f1.Log("添加用户登录信息到文件时出现错误 原因:" + ex.Message);
                return false;
            }
        }
        public bool Delete(int index)
        {
            try
            {
                User_List_Array.RemoveAt(index);
                Write();
                return true;
            }
            catch (Exception ex)
            {
                Form1.f1.Log("删除用户登录信息到文件时出现错误 原因:" + ex.Message);
                return false;
            }
        }
        public string[] Select(string account)
        {
            try
            {
                foreach (JToken jtoken in User_List_Array)
                {
                    if (jtoken["account"].ToString().Equals(account))
                    {
                        return new string[] { jtoken["account"].ToString(), jtoken["userid"].ToString(), jtoken["token"].ToString(), jtoken["planid"].ToString() };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Form1.f1.Log("查询用户登录信息到文件时出现错误 原因:" + ex.Message);
                return null;
            }
        }
        public bool Update(string account, string userid, string token, string planid)
        {
            try
            {
                foreach (JToken jtoken in User_List_Array)
                {
                    if (jtoken["account"].ToString().Equals(account))
                    {
                        jtoken["userid"] = userid;
                        jtoken["token"] = token;
                        jtoken["planid"] = planid;
                        jtoken["UpdateTime"] = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
                        Write();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Form1.f1.Log("更新用户登录信息到文件时出现错误 原因:" + ex.Message);
                return false;
            }
        }







    }
}
