using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace gxy
{
    class daka
    {
        public static string 执行单个打卡任务(string account, string pwd, string state, string address, string country, string province, string city, string jd, string wd)
        {
            if (!LoginCache.Get().isExist(account))
            {
                Form1.f1.Log(string.Format("用户{0}, 不存在历史登录信息, 执行登录.", account));

                string[] Login_Data = V3Login(account, pwd);
                if (Login_Data == null) return "";
                string userid = Login_Data[0];
                string token = Login_Data[1];

                string planid = GetPlanID(account, userid, token);
                if (planid == null) return "";

                LoginCache.Get().Insert(account, userid, token, planid);
                Form1.f1.Log(string.Format("用户{0}, 登录信息保存成功.", account));

                string result = Save(state, account, userid, token, planid, address, country, province, city, jd, wd);
                return result;
            }
            else
            {
                Form1.f1.Log(string.Format("用户{0}, 存在历史登录信息, 进行打卡.", account));

                string[] userinfo = LoginCache.Get().Select(account);

                string result = Save(state, account, userinfo[1], userinfo[2], userinfo[3], address, country, province, city, jd, wd);
                if (!result.Contains("成功"))
                {
                    Form1.f1.Log(result + ". 执行重新登陆...");
                    string[] Login_Data = V3Login(account, pwd);
                    if (Login_Data == null) return "";
                    string userid = Login_Data[0];
                    string token = Login_Data[1];

                    string planid = GetPlanID(account, userid, token);
                    if (planid == null) return "";

                    LoginCache.Get().Update(account, userid, token, planid);
                    Form1.f1.Log(string.Format("用户{0}, 登录信息更新成功.", account));

                    string rresult = Save(state, account, userid, token, planid, address, country, province, city, jd, wd);
                    return rresult;
                }
                return result;
            }
        }

        private static string[] V3Login(string account, string pwd) //登录用户 取userid和token
        {
            //V3 API 登录
            JObject V3_Login_Data = new JObject();
            V3_Login_Data["t"] = EncryptandDecipher.GXY_Login_V3_Encrypt((((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) * 1000).ToString());
            V3_Login_Data["loginType"] = "android";
            V3_Login_Data["phone"] = EncryptandDecipher.GXY_Login_V3_Encrypt(account);
            V3_Login_Data["password"] = EncryptandDecipher.GXY_Login_V3_Encrypt(pwd);

            string request = http.Post("https://api.moguding.net:9000/session/user/v3/login", "", "", "", V3_Login_Data.ToString());
            JObject response = JObject.Parse(request);
            //Console.WriteLine("登录返回:\n" + response.ToString() + "\n\n");
            if (!response["code"].ToString().Equals("200"))
            {
                Form1.f1.Log("用户账户[" + account + "] 登录失败: " + response["msg"]);
                return null;
            }
            return new string[]{
                response["data"]["userId"].ToString(),
                response["data"]["token"].ToString()
            };
        }

        private static string GetPlanID(string account, string userid, string token) //取刚登录完的用户的队列信息 取planid
        {
            JObject plan_post_data = new JObject();
            plan_post_data["state"] = "";

            string sign = EncryptandDecipher.md5jm(userid + "student" + "3478cbbc33f84bd00d75d7dfa69e0daa"); //请求标识
            string request = http.Post("https://api.moguding.net:9000/practice/plan/v3/getPlanByStu", token, sign, "student", plan_post_data.ToString());
            JObject response = JObject.Parse(request);
            //Console.WriteLine("取plan返回:\n" + response.ToString() + "\n\n");
            if (!response["code"].ToString().Equals("200"))
            {
                Form1.f1.Log("用户账户[" + account + "] 取planid失败: " + response["msg"]);
                return null;
            }
            return response["data"][0]["planId"].ToString();
        }

        private static string Save(string state, string account, string userid, string token, string planid, string address, string country, string province, string city, string jd, string wd) //上班或下班, 电话号码, 用户id, 登录token, 用户队列id, 详细地址, 国家, 省份, 城市, 经度, 纬度
        {
            JObject save_post_data = new JObject();
            save_post_data["planId"] = planid;
            save_post_data["device"] = "Android";
            save_post_data["description"] = "";
            save_post_data["type"] = state;
            save_post_data["address"] = address;
            save_post_data["country"] = country;
            save_post_data["province"] = province;
            save_post_data["city"] = city;
            save_post_data["longitude"] = jd;
            save_post_data["latitude"] = wd;

            string sign = EncryptandDecipher.md5jm("Android" + state + planid + userid + address + "3478cbbc33f84bd00d75d7dfa69e0daa");
            string request = http.Post("https://api.moguding.net:9000/attendence/clock/v2/save", token, sign, "student", save_post_data.ToString());
            try 
            {
                JObject response = JObject.Parse(request);
                Console.WriteLine("保存返回:\n" + response.ToString() + "\n\n");
                if (response["code"].ToString().Equals("200"))
                {
                    return string.Format("用户{0}, 打卡成功. 服务器返回时间[{1}]", account, response["data"]["createTime"]);
                }
                else
                {
                    return string.Format("用户{0}, 打卡失败! 原因:{1}", account, response["msg"].ToString());
                }
            }
            catch (Exception ex)
            {
                return string.Format("用户{0}, 打卡失败! 原因:{1}", account, ex.ToString());
            }
        }





    }
}
