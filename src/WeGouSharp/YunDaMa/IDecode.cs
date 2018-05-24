namespace WeGouSharpPlus.YunDaMa
{
    /// <summary>
    /// 打码接口
    /// </summary>
    public interface IDecode
    {
        
        string OnlineDecode(string imageLocation);

        void SetTryLimit(int max);


    }
}