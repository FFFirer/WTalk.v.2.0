using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using WTalk.Domain;

namespace WTalk.Client.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        /// <summary>
        /// 所有聊天未读消息
        /// </summary>
        private int allchatwaitreads { get; set; }
        public int AllChatWaitReads
        {
            get { return this.allchatwaitreads; }
            set
            {
                if (allchatwaitreads == value) { return; }
                allchatwaitreads = value;
                Notify("allchatwaitreads");
            }
        }
        /// <summary>
        /// 所有好友添加数
        /// </summary>
        private int allfriendwaitreads { get; set; }
        public int AllFriendWaitReads
        {
            get { return this.allfriendwaitreads; }
            set
            {
                if (allfriendwaitreads == value) { return; }
                allfriendwaitreads = value;
                Notify("allfriendwaitreads");
            }
        }
        /// <summary>
        /// 所有添加好友数
        /// </summary>
        private int allnoticewaitreads { get; set; }
        public int AllNoticeWaitReads
        {
            get { return this.allnoticewaitreads; }
            set
            {
                if (allnoticewaitreads == value) { return; }
                allnoticewaitreads = value;
                Notify("allnoticewaitreads");
            }
        }
    }

    //聊天列表
    public class ChatList : ObservableCollection<Chat> { }
    //好友列表
    public class FriendList : ObservableCollection<Friend> { }
    //通知列表
    public class NoticeList : ObservableCollection<Notice> { }
    //聊天消息记录
    public class MsgList : ObservableCollection<PerMsg> { }

    //每个聊天的信息
    public class Chat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public MsgList Msgs { get; set; }
        private int waitreadnum { get; set; }
        public int WaitReadNum
        {
            get { return waitreadnum; }
            set
            {
                if (this.waitreadnum == value) { return; }
                this.waitreadnum = value;
                Notify("waitreadnum");
            }
        }
    }

    //每一条聊天记录
    public class PerMsg
    {
        public string SenderName { get; set; }
        public string Msg { get; set; }
        public PerMsg() { }
        public PerMsg(string name, string msg)
        {
            this.SenderName = name;
            this.Msg = msg;
        }
    }

    //好友
    public class Friend
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
        public Status status { get; set; }
    }

    //好友申请通知
    public class Notice
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Status status { get; set; }
    }
}
