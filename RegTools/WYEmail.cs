﻿using System;
using HttpCodeLib;
using System.IO;

namespace RegTools
{
    class Email163
    {
        #region 变量

        HttpHelpers  helper = null ;
        HttpItems items = null ;
        HttpResults hr = null;
        String Cookies =string.Empty ;
        String Proxy = string.Empty;

        public delegate string GetImgCode(byte[] bytes);
        public GetImgCode getImgCode;
        #endregion 变量

        string winx1_1;
        
        #region 构造
        public Email163()
        {
            winx1_1 = (((long)new Random().Next(100) * 10e4) + 10e6).ToString() + ((long.Parse(new XJHTTP().GetTimeByJs()) / 1000).ToString());
            helper = new HttpHelpers();
            items = new HttpItems();
            
        }

        #endregion 构造


        #region 功能

        /// <summary>
        /// 检查某用户名可否被注册
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool CheckUserName(string username)
        {
            try
            {
                string url = String.Format("http://reg.163.com/services/checkSsnAll?isret=1&username={0}", username);
                items = new HttpItems();
                items.URL = url;
                //items.Container = cc;
                //items.Cookie = Cookies;
                // items.ProxyIp = proxy ;
                hr = helper.GetHtml(items);
                string reHtml = hr.Html.Replace("\r\n", "").Replace("\t", "").Replace("\n", "");
                reHtml = reHtml.Trim();
                //Cookiesss += hr.Cookie;
                if (reHtml.Contains ( "200OK.0"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        public void setProxy(string proxyip)
        {
            this.Proxy = proxyip;
        }
        public bool RegUserPass(string user,string pass,ref string status )
        {
            try
            {
                string codeID= string.Empty ;
                string postData = string.Empty;
                string Vcode = string.Empty;
                 byte[] codebytes = null;
                string[] pros = { "db","xyq","xy2","ty","dh2","bw","xy3","xqn",
                                 "x3","zmq","dtws2","lj","wh","jl","zh","ff",
                                    "tuji","wxdj","sc2","y3","my","dhxy","ldxy"};

                string pro = pros[new Random ().Next (pros.Length )];
                Cookies = "";
                status = string.Empty ;

                if(!CheckUserName (user))
                {
                    status = "检查用户名可注册性返回失败";
                    return false;
                }


                //获取初始　JSESSIONID,SID两个唯一标识，返回的Cookie里面
                string url = String.Format("http://reg.163.com/reg/innerDomainReg.do?product="+pro+"&url=http%3A%2F%2F"+pro+".163.com&loginurl=http%3A%2F%2F"+pro+".163.com");
                items = new HttpItems();
                items.URL = url;
                //items.Container = cc;
                items.Cookie = Cookies;
                items.ProxyIp = Proxy;
                hr = helper.GetHtml(items, ref Cookies);
                string  reHtml = hr.Html.Replace("\r\n", "").Replace("\t", "").Replace("\n", "");
                //reHtml = reHtml.Trim();

        GETVCODE:
                //获取验证码ID

                url = String.Format("http://reg.163.com/services/getid");
                items = new HttpItems();
                  items.URL = url;
                //items.Container = cc;
                items.Cookie = Cookies;
                 items.ProxyIp = Proxy;
                hr = helper.GetHtml(items, ref Cookies);
                reHtml = hr.Html.Replace("\r\n", "").Replace("\t", "").Replace("\n", "");
                reHtml = reHtml.Trim();

                codeID= reHtml;

                //获取验证码数据

                url = String.Format("http://reg.163.com/services/getimg?v=1448380241064&num=6&type=2&id={0}", codeID);
                 items = new HttpItems();
                items.URL = url;
                ///items.Container = cc;
                items.Cookie = Cookies;
                items.ResultType = ResultType.Byte;
                items.ProxyIp = Proxy;
                hr = helper.GetHtml(items, ref Cookies);

                codebytes = hr.ResultByte;
                //获取验证码
                 if(getImgCode ==null || codebytes ==null ||string.IsNullOrEmpty  (Vcode =getImgCode (codebytes )) )
                {
                    status = "验证码识别有问题："+Vcode;
                    return false;
                }

                //正式提交注册数据
                string winx1_2 = (((long)new Random().Next(100) * 10e4) + 10e6).ToString() + ((long.Parse(new XJHTTP().GetTimeByJs()) / 1000).ToString());
                //username = "tempemail2";
                //pwd = "tempemail2163";
                //vcode = "29cbgx";
                //codeId = "818e907d3eda65edfead7ec0f3f4d3ba1b45a181";
                string codez = winx1_1 + "_" + winx1_2;
                url = String.Format("https://reg.163.com/reg/innerDomainRegSubmit.do");
                postData = String.Format(
                    "url=http://"+pro+".163.com&product="+pro+"&loginurl=http%3A%2F%2F"+pro+".163.com&fromUrl=http%253A%252F%252Freg.163.com%252Freg%252Freg.jsp%253Fproduct%253D"+pro+"%2526url%253Dhttp%25253A%25252F%25252F"+pro+".163.com%2526loginurl%253Dhttp%25253A%25252F%25252F"+pro+".163.com&u1=0&codez={0}&radomPassID={1}&username={2}&domain=%40163.com&password={3}&cpassword={4}&radomPass={5}&agree=on"
                    , codez, codeID , user, pass, pass, Vcode);

                items = new HttpItems();
                items.URL = url;
                items.ProxyIp = Proxy;
                items.Method = "POST";
                items.Postdata = postData;
                items.Referer = "http://reg.163.com/reg/innerDomainReg.do?product="+pro+"";
                items.Cookie = "_ntes_nnid=; _ntes_nuid=;URS_Analyze=1; reg_info=" + new Random().Next(11111111, 100000000).ToString() + ";" + Cookies;
                //items.Cookie = "_ntes_nnid=; _ntes_nuid=;URS_Analyze=1; reg_info=366303;JSESSIONID=decg0XMdMALNThAXai-ev;REG_ANALYSIS=85faf129-494a-480e-b2e8-13d3b46138a6;SID=403b5163-07a1-4493-a20e-8ed11e31d144;";
                items.Allowautoredirect = true ;
                hr = helper.GetHtml(items);
                reHtml = hr.Html.Replace("\r\n", "").Replace("\t", "").Replace("\n", "");
                reHtml = reHtml.Trim();
                if (reHtml.Contains("您输入的图片中的文字不正确！"))
                {
                    goto GETVCODE;
                }
                else if (reHtml.Contains("正在登录，请稍等"))
                {
                    File.AppendAllText(Environment.CurrentDirectory + @"\finishUsers.txt", string.Format("{0}----{1}\r\n", user, pass));

                    status +="注册成功\t";
                    return true;
                    //MessageBox.Show("注册成功");
                }
                else
                {
                    if(reHtml .Contains ("短信验证"))
                    status += "短信验证" ;
                    else
                    {
                        status +=reHtml ;
                    }
                    return false;
                    //MessageBox.Show(reHtml);
                }
            }
            catch (Exception ex )
            {
             }
            return false;
        }



        #endregion 功能



    }
}
