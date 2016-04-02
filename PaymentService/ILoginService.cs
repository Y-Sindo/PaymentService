using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PaymentService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ILoginService”。
    [ServiceContract]
    public interface IService
    {
        [OperationContract]  //post
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "login") ]    // Add this HTTP GET attribute/directive, use default format
            ResponseData login(RequestData rData);

        [OperationContract]
        [WebGet(UriTemplate = "getBill?id={id}")]             // get 
            string getBill(int id);

}


}
