using WeGouSharp.Model;

namespace WeGouSharp.YunDaMa
{
    /// <summary>
    /// 打码接口
    /// </summary>
    public interface IDecoder
    {
        
        string Decode(string imageLocation,CaptchaType captchaType);

        void SetTryLimit(int max);


    }
}