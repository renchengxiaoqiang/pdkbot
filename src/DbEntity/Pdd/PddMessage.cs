using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdkBot.DbEntity.Pdd
{
    public class Target
    {
        public string mall_id { get; set; }
        public string mms_uid { get; set; }
    }



    public class Body
    {
        public string targetMmsId { get; set; }
        public string uid { get; set; }
        public string targetMallId { get; set; }
        public int trusteeshipMode { get; set; }
        public bool? canActiveManually { get; set; }
        public string description { get; set; }
        public string signature { get; set; }
        public long updatedAt { get; set; }
        public long validBefore { get; set; }
        public object pendingConfirmData { get; set; }
    }

    public class From
    {
        public string uid { get; set; }
        public string role { get; set; }
    }

    public class To
    {
        public string uid { get; set; }
        public string role { get; set; }
        public string mall_id { get; set; }
    }

    public class Push_biz_context
    {
        public int msg_category { get; set; }
    }

    public class Message
    {
        //public long ts { get; set; }
        public From from { get; set; }
        public To to { get; set; }
        public string client_msg_id { get; set; }
        public int type { get; set; }
        public string content { get; set; }
        public int version { get; set; }
        public string msg_id { get; set; }
        public string pre_msg_id { get; set; }
        public string nickname { get; set; }
        public Push_biz_context push_biz_context { get; set; }
        public string rawContent { get; set; }
        public string _textProcessContent { get; set; }


        public Data data { get; set; }
    }

    public class Data
    {
        public string user_id { get; set; }
        public string user_last_read { get; set; }
        public string min_supported_msg_id { get; set; }
    }

    public class InnerData
    {
        public Message message { get; set; }
        public int chat_type_id { get; set; }
    }

    public class Push_data
    {
        public int seq_type { get; set; }
        public int seq_id { get; set; }
        public List<InnerData> data { get; set; }
        public int base_seq_id { get; set; }
    }

    public class Payload
    {
        public int push_type { get; set; }
        public Push_data push_data { get; set; }
        public long target_id { get; set; }


        public string bizType { get; set; }
        public string subType { get; set; }
        public Body body { get; set; }

        public string result { get; set; }
        public string response { get; set; }
        public Message message { get; set; }
        public string request_id { get; set; }
        public Target target { get; set; }
    }

    public class PddReceiveMessage
    {
        public int actionId { get; set; }
        public Payload payload { get; set; }
    }
}
