using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace _2_PaymentService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //测试
        [OperationContract]
        [WebInvoke(UriTemplate ="getBill", ResponseFormat = WebMessageFormat.Json,Method ="POST")]
        string getBill(GetBillPara getBillPara);
    
        //注册
        [OperationContract]
        [WebInvoke(UriTemplate = "register1", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        Rs_Pki register1(string username);//username ,pki

        [OperationContract]
        [WebInvoke(UriTemplate = "register2", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int register2(User_Cyphertext user_cyphertext);// username, loginPassword#PayPassword

        //登陆
        [OperationContract]
        [WebInvoke(UriTemplate = "login1", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        Rs_Pki login1(string username);//username ,pki

        [OperationContract]
        [WebInvoke(UriTemplate = "login2", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int login2(User_Cyphertext user_cyphertext);//username,loginPassword
        
        //模式转换
        [OperationContract]
        [WebInvoke(UriTemplate = "turnToPasswordFreeMode", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int turnToPasswordFreeMode(User_Cyphertext user_cyphertext);//username,paymentPassword

        [OperationContract]
        [WebInvoke(UriTemplate = "exitPasswordFreeMode", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int exitPasswordFreeMode(User_Cyphertext user_cyphertext);//username,loginPassword

        //查询信息
        [OperationContract]
        [WebInvoke(UriTemplate = "getMode", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int getMode(string username);

        [OperationContract]
        [WebInvoke(UriTemplate = "getBalance", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        double getBalance(string username);

        [OperationContract]
        [WebInvoke(UriTemplate = "getBills", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        Bill[] getBills(string username);

        //支付
        [OperationContract]
        [WebInvoke(UriTemplate = "payWithPassword", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int payWithPassword(PayInfo payInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "payWithoutPassword", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        int payWithoutPassword(PayInfo payInfo);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class GetBillPara
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string username { get; set; }
    }

    [DataContract]
    public class Rs_Pki
    {
        [DataMember]
        public int rs { get; set; }
        [DataMember]
        public string pki { get; set; }
    }

    [DataContract]
    public class User_Cyphertext
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string cyphertext { get; set; }
    }

    [DataContract]
    public class Bill {
   
        [DataMember]
        public int bill_id { get; set; }

        [DataMember]
        public string user_id { get; set; }

        [DataMember]
        public string merchant_id { get; set; }

        [DataMember]
        public DateTime date { get; set; }

        [DataMember]
        public double amount { get; set; }

        [DataMember]
        public string detail { get; set; }
    }

    [DataContract]
    public class PayInfo
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string cyphertext { get; set; }
        [DataMember]
        public string detail { get; set; }
    }



    
}
