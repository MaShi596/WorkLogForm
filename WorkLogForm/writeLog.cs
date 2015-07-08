﻿using System;
using System.Drawing;
using System.Windows.Forms;
using WorkLogForm.WindowUiClass;
using WorkLogForm.Service;
using ClassLibrary;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using WorkLogForm.CommonClass;

namespace WorkLogForm
{
    public partial class writeLog : Form
    {


       
        private BaseService baseService = new BaseService();
        private WkTUser user;
        private DateTime logDate;
        private List<WkTUser> sharedUser;
        private bool isView = false;
        /// <summary>
        /// 界面用于编写还是展示
        /// </summary>
        public bool IsView
        {
            get { return isView; }
            set { isView = value; }
        }

        private String commentPersonName;
        /// <summary>
        /// 传送评论人信息也就是当前登陆系统的人
        /// </summary>
        public String CommentPersonName
        {
            get { return commentPersonName; }
            set { commentPersonName = value; }
        }

        private bool isComment = false;
        /// <summary>
        /// 用否需要评论（现在规定只要是展示就需要可以评论）
        /// </summary>
        public bool IsComment
        {
            get { return isComment; }
            set { isComment = value; }
        }
        public List<WkTUser> SharedUser
        {
            get { return sharedUser; }
            set { sharedUser = value; }
        }
        public DateTime LogDate
        {
            get { return logDate; }
            set { logDate = value; }
        }
        /// <summary>
        /// 当前日志是谁写的
        /// </summary>
        public WkTUser User
        {
            get { return user; }
            set { user = value; }
        }
        public writeLog()
        {
            InitializeComponent();
        }
        private void writeLog_Load(object sender, EventArgs e)
        {
            initialWindow();
            initData();
            schedule_listView.Focus();
            //this.Location = new Point((Screen.GetBounds(this).Width - this.Width) / 2, (Screen.GetBounds(this).Height - this.Height) / 2);
        }     
        #region 自定义窗体初始化方法
        private void initialWindow()
        {
            creatWindow.SetFormRoundRectRgn(this, 15);
            creatWindow.SetFormShadow(this);
            if (this.isComment == true)
            {
                label1.Dispose();
                label4.Font = new Font(label4.Font.FontFamily, (float)18, FontStyle.Bold);
                label4.Location = new Point(630, 44);
                label4.Text = user.KuName+"的日志-" + logDate.Year + "年" + logDate.Month + "月" + logDate.Day + "日";
                
                schedule_listView.Dispose();
                button1.Dispose();
               
                button3.Dispose();
                htmlEditor1.ShowToolBar = false;
                htmlEditor1.ReadOnly = true;
                this.label3.Text = "评  论";
                this.comment_listView.Parent = RichengPanel;
                this.comment_listView.Location = this.schedule_listView.Location;
                this.comment_listView.Size = this.schedule_listView.Size;
                this.schedule_listView.Visible = false;
                

                comment_listView.Visible = true;
                comment_textBox.Visible = true;
                comment_button.Visible = true;
            }
            //else if (this.isView == true)
            //{
            //    label1.Dispose();
            //    label4.Font = new Font(label4.Font.FontFamily, (float)21.75, FontStyle.Bold);
            //    label4.Location = new Point(647, 104);
            //    label4.Text = "查看日志-" + logDate.Year + "年" + logDate.Month + "月" + logDate.Day + "日";
            //    label3.Dispose();
            //    label2.Dispose();
            //    schedule_listView.Dispose();
            //    button1.Dispose();
            //    //button2.Dispose();
            //    button3.Dispose();
            //    htmlEditor1.ShowToolBar = false;
            //    htmlEditor1.ReadOnly = true;
            //    htmlEditor1.Location = new Point(57, 90);
            //    htmlEditor1.Size = new Size(890, 425);
            //    comment_listView.Top += 99;
            //    comment_listView.Visible = true;
            //}
           
            //this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 5, Screen.PrimaryScreen.WorkingArea.Height / 8);
        }
        private void initData()
        {
            if (logDate == null || logDate.Equals(new DateTime(0)))
            {
                logDate = DateTime.Now;
            }
            IList staffLogList = baseService.loadEntityList("from StaffLog where State=" + (int)IEntity.stateEnum.Normal + " and WriteTime >=" + logDate.Date.Ticks + " and Staff=" + user.Id);
           
            if (staffLogList != null && staffLogList.Count > 0)
            {
                StaffLog s = (StaffLog)staffLogList[0];
                htmlEditor1.BodyInnerHTML = s.Content;
                htmlEditor1.Tag = staffLogList[0];
                SharedUser = new List<WkTUser>();
                SharedUser.AddRange(s.SharedStaffs);
                initCommentList(s.Comments);
            }
            if (IsComment != true)
            {
                //偏好设置
                IList staffHobbyList = baseService.loadEntityList("from Hobby where State=" + (int)IEntity.stateEnum.Normal + " and Staff=" + user.Id + " and TypeFlag=" + (int)Hobby.hobbyTypeEnum.RiZhi);
                if (staffHobbyList != null && staffHobbyList.Count > 0)
                {
                    Hobby s = (Hobby)staffHobbyList[0];
                    SharedUser = new List<WkTUser>();
                    SharedUser.AddRange(s.SharedStaffs);
                }

                //当用于编写日志作用时候加载日程

                long nextDay = logDate.Date.Ticks + new DateTime(1, 1, 2).Date.Ticks;
                IList scheduleList = baseService.loadEntityList("from StaffSchedule where State=" + (int)IEntity.stateEnum.Normal + " and Staff=" + user.Id + " and ScheduleTime>=" + logDate.Date.Ticks + " and ScheduleTime<" + nextDay + " order by ScheduleTime asc");
                initScheduleList(scheduleList);
            }
            
        }
        #endregion
        #region 最小化关闭按钮
        private void min_pictureBox_MouseEnter(object sender, EventArgs e)
        {
            min_pictureBox.BackgroundImage = WorkLogForm.Properties.Resources.Minenter;
        }
        private void min_pictureBox_MouseLeave(object sender, EventArgs e)
        {
            min_pictureBox.BackgroundImage = null;
        }
        private void min_pictureBox_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void close_pictureBox_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void close_pictureBox_MouseEnter(object sender, EventArgs e)
        {
            close_pictureBox.BackgroundImage = WorkLogForm.Properties.Resources.Closeenter;
        }
        private void close_pictureBox_MouseLeave(object sender, EventArgs e)
        {
            close_pictureBox.BackgroundImage = null;
        }
        #endregion
        #region 窗体移动代码
        private int x_point, y_point;
        private void writeLog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.x_point = e.X;
                this.y_point = e.Y;
            }
        }

        private void writeLog_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.Location.Y > 0)
            {
                Top = MousePosition.Y - y_point;
                Left = MousePosition.X - x_point;
            }
            else if (e.Button == MouseButtons.Left && e.Y > this.y_point)
            {
                Top = MousePosition.Y - y_point;
                Left = MousePosition.X - x_point;
            }
        }
        #endregion

        private void initCommentList(IList<Comments> scList)
        {
            if (scList != null && scList.Count > 0)
            {
                comment_listView.Items.Clear();
                foreach (Comments s in scList)
                {
                    ListViewItem item = new ListViewItem();
                    item.Font = new Font(comment_listView.Font.FontFamily, 9, FontStyle.Regular);
                    item.Text = s.Content.Trim();
                    ListViewItem.ListViewSubItem time = new ListViewItem.ListViewSubItem();
                    ListViewItem.ListViewSubItem commentPerson = new ListViewItem.ListViewSubItem();
                    DateTime dateTime = new DateTime(s.TimeStamp);
                    time.Text = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
                    commentPerson.Text = s.CommentPersonName.Trim();
                    item.SubItems.Add(commentPerson);
                    item.SubItems.Add(time);
                    comment_listView.Items.Add(item);
                }
            }
            comment_listView.Columns[0].Width = 665;
        }
        private void initScheduleList(IList scList)
        {
            if (scList != null && scList.Count > 0)
            {
                schedule_listView.Items.Clear();
                
                foreach (StaffSchedule s in scList)
                {
                    ListViewItem item = new ListViewItem();
                    item.Font = new Font(schedule_listView.Font.FontFamily, 9, FontStyle.Regular);
                    ListViewItem.ListViewSubItem time = new ListViewItem.ListViewSubItem();
                    ListViewItem.ListViewSubItem content = new ListViewItem.ListViewSubItem();
                    time.Text = CNDate.getTimeByTimeTicks(new DateTime(s.ScheduleTime).TimeOfDay.Ticks);
                    content.Text = s.Content;
                    ListViewItem.ListViewSubItem check = new ListViewItem.ListViewSubItem();
                    ListViewItem.ListViewSubItem number = new ListViewItem.ListViewSubItem();
                    number.Text = "1";

                    //item.SubItems.Add(number); 多余的代码
                    item.SubItems.Add(time);
                    item.SubItems.Add(content);
                    
                    schedule_listView.Items.Add(item);
                    
                    
                }
            }
        }
        private void schedule_listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItem item = e.Item;
            if (item.Checked)
            {
                htmlEditor1.BodyInnerHTML += "<p>" + item.SubItems[1].Text + "-" + item.SubItems[2].Text + "。</p>";
            }
            else
            {
               // htmlEditor1.BodyInnerHTML = htmlEditor1.BodyInnerHTML!=null?htmlEditor1.BodyInnerHTML.Replace(item.SubItems[1].Text + "-" + item.SubItems[2].Text + "。", ""):"";
            }
        }
        /// <summary>
        /// 上传完毕后删除生成的临时文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                String fileName = Encoding.Default.GetString(e.Result);
                String tempFilePath = Application.StartupPath.ToString() + "\\temp\\" + fileName;
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            #region 上传图片并生成新html
            Regex r = new Regex("<IMG[\\s\\S]*?>");
            MatchCollection mc = r.Matches(htmlEditor1.BodyInnerHTML);
            ///string[] mc = CommonClass.HtmlUtility.GetElementsByTagName(htmlEditor1.BodyInnerHTML, "IMG");

            String html = htmlEditor1.BodyInnerHTML;
            Uri endpoint = new Uri(Securit.DeDES(IniReadAndWrite.IniReadValue("fileManage", "filePath")));
            for (int i = 0; i < mc.Count; i++)
            {
                if (mc[i].Value.Contains("src=\"http://"))
                {
                    continue;
                }
                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(uploadCompleted);
                    String imgHtml = mc[i].Value;

                    string inPath = HtmlUtility.GetTitleContent(imgHtml, "img", "src");  //imgHtml.Substring(startIndex, imgHtml.LastIndexOf("\"") - startIndex);

                    try
                    {
                        String newName = user.Id + "_" + DateTime.Now.Ticks + inPath.Substring(inPath.LastIndexOf("."));
                        String tempPath = Application.StartupPath.ToString() + "\\temp\\" + newName;
                        File.Copy(inPath, tempPath, true);

                        myWebClient.UploadFileAsync(endpoint, tempPath);

                        String newString2 = imgHtml.Remove(imgHtml.IndexOf("src"), inPath.Length + 6);//.Remove(imgHtml.LastIndexOf('>'))+" src=\"" + Securit.DeDES(IniReadAndWrite.IniReadValue("fileManage", "savePath")) + newName + "\">";
                        string newString1 = newString2.Remove(newString2.LastIndexOf('>'));
                        string newString = newString1 + " src=\"" + Securit.DeDES(IniReadAndWrite.IniReadValue("fileManage", "savePath"))+@"LogPic\" + newName + "\">";
                        //html

                        html = html.Replace(mc[i].Value, newString);

                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString());
                        return;
                    }
                }
            }
            #endregion
            StaffLog staffLog = null;
            if (htmlEditor1.Tag == null)
            {
                staffLog = new StaffLog();
            }
            else if (htmlEditor1.Tag != null)
            {
                staffLog = (StaffLog)htmlEditor1.Tag;
            }
            staffLog.Content = html;
            staffLog.State = (int)IEntity.stateEnum.Normal;
            staffLog.WriteTime = DateTime.Now.Ticks;
            staffLog.TimeStamp = DateTime.Now.Ticks;
            staffLog.Staff = user;
            staffLog.SharedStaffs = sharedUser;
           
            try
            {
                if (staffLog.Id == 0)
                {
                   object be =  baseService.saveEntity(staffLog);
                   staffLog.Id = int.Parse(be.ToString());
                }
                else if (staffLog.Id != 0)
                {
                    baseService.SaveOrUpdateEntity(staffLog);
                }

                #region 向服务中发送数据

                try
                {
                    KjqbService.Service1Client ser = new KjqbService.Service1Client();
                    if (sharedUser != null && sharedUser.Count > 0)
                    {

                        foreach (WkTUser u in sharedUser)
                        {
                            KjqbService.LogInService ll = new KjqbService.LogInService();
                            ll.LogId = staffLog.Id;
                            ll.WriteUserId = this.user.Id;
                            ll.ShareUserId = u.Id;
                            ll.TimeStamp = DateTime.Now.Ticks;
                            ser.SaveInLogListInService(ll);
                        }

                    }
                }
                catch 
                {
                
                }
                #endregion

            }
            catch
            {
                MessageBox.Show("保存失败！");
                return;
            }
            MessageBox.Show("保存成功！");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
       
        private void button3_Click(object sender, EventArgs e)
        {
            InterimHead interimHead = new InterimHead();
            interimHead.User = this.user;
            interimHead.ParentForm1 = this;
            interimHead.ShowDialog();
        }

        /// <summary>
        /// 没有用！！！！！不知道为啥！！！！
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void htmlEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                return;
            }
        }

        private void comment_button_Click(object sender, EventArgs e)
        {
            this.panelofWriteComment.Visible = true;

        }

        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.RichengPanel.Visible == true)
            {
                this.RichengPanel.Visible = false;
                this.pictureBox1.Image = WorkLogForm.Properties.Resources.ShowRichengInWriteLog;
                this.htmlEditor1.Height = 580;
                this.panel2.Location = new Point (this.panel2.Location.X,this.htmlEditor1.Location.Y+htmlEditor1.Height+5);

            }
            else if (this.RichengPanel.Visible == false)
            {
                this.RichengPanel.Visible = true;
                this.pictureBox1.Image = WorkLogForm.Properties.Resources.HideRichengInWriteLog;
                this.htmlEditor1.Height = 410;
                this.panel2.Location = new Point(this.panel2.Location.X, this.htmlEditor1.Location.Y + htmlEditor1.Height + 5);

            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (this.comment_textBox.Text.Trim().Length > 0)
            {

                StaffLog staffLog = (StaffLog)htmlEditor1.Tag;
                Comments comment = new Comments();
                comment.Content = comment_textBox.Text.Trim();
                comment.CommentPersonName = this.CommentPersonName;
                comment.State = (int)IEntity.stateEnum.Normal;
                comment.TimeStamp = DateTime.Now.Ticks;
                staffLog.Comments.Add(comment);
                baseService.SaveOrUpdateEntity(staffLog);
                initCommentList(staffLog.Comments);
                this.comment_textBox.Text = "";


                #region 向服务中发送数据
                try
                {
                    KjqbService.Service1Client ser = new KjqbService.Service1Client();
                    if (comment.CommentPersonName != "")
                    {
                        KjqbService.CommentInService ll = new KjqbService.CommentInService();
                        ll.LogId = staffLog.Id;
                        ll.CommentUserName = this.CommentPersonName;
                        ll.LogUserId = staffLog.Staff.Id;
                        ll.TimeStamp = DateTime.Now.Ticks;
                        ser.SaveInCommentListInService(ll);

                    }
                }
                catch
                {


                }
                #endregion 

                this.labelMessageBox1.MessageageShow("评论成功！");

            }
            else
            {
                this.labelMessageBox1.MessageageShow("您的评论内容为空……");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.panelofWriteComment.Visible = false;
        }
      

    }
}
