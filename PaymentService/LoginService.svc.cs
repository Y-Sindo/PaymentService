using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PaymentService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“LoginService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 LoginService.svc 或 LoginService.svc.cs，然后开始调试。
    [DataContract]
    public class RequestData
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string password { get; set; }
    }
    [DataContract]
    public class ResponseData
    {
        [DataMember]
        public string loginResult { get; set; }
        
        
    }

    
    public class Service : IService
    {
        public string getBill(int id)
        {
            return "Book 25.00 1";
        }
        
        public ResponseData login(RequestData rData)
        {
            return new ResponseData { loginResult = rData.username + " " + rData.password };
        }
    }

}
