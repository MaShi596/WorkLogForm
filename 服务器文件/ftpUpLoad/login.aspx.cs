using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string key in Request.Files.AllKeys)
        {
            HttpPostedFile file = Request.Files[key];
            try
            {
                string strname = file.FileName;
                if (strname.Contains("xiaoban_"))
                {
                     file.SaveAs("C:\\ftpUpLoad\\UpLoadFile\\xiaobanpic\\" + file.FileName);
                      
                }
                else
                {
                     file.SaveAs("C:\\ftpUpLoad\\UpLoadFile\\LogPic\\" + file.FileName);
                }
                //文件保存并返回相对路径地址
                Response.ContentType = "text/plain";
                Response.Write(file.FileName);
                Response.End();
            }
            catch (Exception)
            {
            }
        }
    }
}