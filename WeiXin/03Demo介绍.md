- 打开`src/Senparc.Weixin.MP.Sample.vs2017`下的解决方案文件
- 运行Web项目，和官网类似：`sdk.weixin.senparc.com`
- 配置文件
- 在盛派网络小助手中消息:输入openid等文本，点击菜单，返回文本，返回图片，返回音乐，返回图文等。对接受信息的处理放在了`CommonService`这个类库里了。


```
//需要上下文
public class CustomMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
{
    public CustomMessageContext()
    {
        base.MessageContextRemoved += CustomMessageContext_MessageContextRemoved;
    }

    void CustomMessageContext_MessageContextRemoved(object sender, Senparc.Weixin.Context.WeixinContextRemovedEventArgs<IRequestMessageBase, IResponseMessageBase> e)
    {
        var messageContext = e.MessageContext as CustomMessageContext;
        if(messageContext == null) return;
    }
}

//需要自定义的Handler
public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
{
    private string appId = "";
    private string appSecret = "";
    public static Dictionary<stirng, string> TemplateMessageCollection = new Dictionary<string, string>();

    //PostModel用来接收所有的消息类型
    public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecourdCount = 0) : base(inputStream, postModel, maxRecordCount)
    {
        //实际可以在MessageHandler<MessageContext>.GlobalWeixinContext.ExpiredMinutes =3中设置
        WeixinContext.ExporeMinutes = 3;

        if(!string.IsNullOrEmpty(postModel.AppId))
        {
            appId = postModel.AppId;
        }

        //默认情况下不使用消息去重
        base.OmitRepeatMessageFunc = requestMessage =>
        {
            var textRequestMessage = requestMessage as RequestMessageText;
            if(textRequestMessage != null && textRequestMessage.Content == "容错")
            {
                return false;
            }
            return true;
        };
    }

    public CustomMessageHandler(RequestMessageBase requestMessage) : base(requestMessage)
    {

    }

    public override void OnExecuting()
    {
        //CurrentMessageContext获取当前上下文
        //CurrentMessageContext.StorageData获取当前上下问的数量
        if(CurrentMessageContext.StorageData == null)
        {
            CurrentMessageContext.StorageData = 0;
        }
        base.OnExecuting();
    }

    public override void OnExecuted()
    {
        base.OnExecuted();
        CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
    }

    public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
    {
        //调用基方法返回响应消息
        var defaultResponseMessage = base.CreateResponseMessage<ResponseMessageText>();

        var requestHandler = requestMessage.StartHandler()
            .Keyword("约束", () => {
                defaultResponseMessage.Content = "";
                return defaultResponseMessage;
            })
            .Keywords(new[]{"",""}, () => {
                return defaultResponseMessage;
            })
            .Keyword("TM",() => {
                //从请求信息中可以获取用户的openid
                var oepnId = requestMessage.FromUserName;
                var checkCode = Guid.NewGuid().ToString("n").Substring(0,3);
                TemplateMessageCollection[checkCode] = openId;
                defaultResponseMessage.Content = "";
                return defaultResoponseMessage;
            })
            .Keyword("OPENID", () => {

                var openId = requestMessage.FromUserName;
                //AdvancedAPIs.UserApi.Info用来封装用户信息
                var userInfo = AdvancedAPIs.UserApi.Info(appId, openId, Language.zh_CN);
                defaultResponseMessage.Content = "";
                return defaultResponsMessage;
            })
            .Keyword("EX", () => {
                var ex = new WeixinException();//有专门的异常类
                defaultResponsMessage.Content = "";
                return defaultResponsMessage;
            })
            .Keyword("MUTE", () => {
                //方式一
                return new SuccessResponsMessage();

                //方式二
                return base.CreateResponseMessage<ResponsMessageNoResponse>();

                //方式三
                base.TextResponseMessage = "success";
                return null;
            })
            .Default(()=>{

            })
            .Regex(@"^\d+#\d+$",()=>{

            });


        return requestHandler.GetResponseMessage() as IResponseMessageBase;
    }

    public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
    {
        var locationSerivce = new LocationServce();
        var responseMessage = locationService.GetResponsMessate(requestMessage as RequestMessageLocation);
        return responsMessage;
    }

    public overeride IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage)
    {

    }

    public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
    {

    }

    public override IResponsMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
    {

    }

    public override IResponseMessageBase OnVideoRequest(RequestMessageVideo reuqestMessage)
    {

    }

    public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
    {

    }

    public override IResponsMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
    {

    }

    public override IR4esponseMessageBase DefaultResponsMessage(IRequestMessageBase requestMessage)
    {

    }

    public override IResponseMessageBase OnUnknownTypeRequest(ReuqestMessageUnknownType requestMessage)
    {

    }
}
```

- 测试异步消息，在手机上输入tm, 然后在网页上输入验证码，请求来到如下控制器方法
  
```
public class AsyncMethodsController : Controller
{
    private string appId;
    private string appSecret;

    IOptions<SenparcWeiXiniSetting> _senparcWeixinSetting;

    public AsyncMethodsController(IOptions<SenparcWeixinSetting> senparcWeixinSetting)
    {
        _senparkWeixinSetting = senparcWeixinSetting;
        appId = _senparcWeixinSetting.Value.WeixinAppId;
        appSecret = _senparcWeixinSetting.Value.WeixinAppSecret;
    }

    public async Task<ActionResult> TemplateMessageTest(string checkCode)
    {
        //应用服务器返回的状态码也被放到了服务器的一个字典中,key是checkCode,value是用户的openId
        var openId = CustomerMessageHandler.TemplateMessageCollection.ContainsKey(checkCode) ? CustomMessageHandler.TemplateMessageCollection[checkCode] : null;

        if(openId == null)
        {
            return Content("验证码已经过期");
        }
        else
        {
            CustomMessageHandler.TemplateMessageCollection.Remove(checkCode);

            //准备模板信息
            var templateId = "";
            var testData = new{
                first = new TemplateDataItem(),
                keyword1= new TemplateDataItem(openId),
                keyword2 = new TemplateDataItem(""),
                keyword3 = new TemplateDataItem(DataTime.Now.ToString("0")),
                remark = new TemplateDataItem()
            };

            var result = await TemplateApi.SendTemplateMessageAsync(appId, openId, templateId, "pages/index/index", testData);
            return Content("");
        }
    }
}
```

- 测试缓存：http://sdk.weixin.senparc.com/Cache/Test

```

```