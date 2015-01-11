#define DOTNET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using System.Diagnostics;
using BIN.Security;
using BIN.DataAccess.Connection;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
namespace BIN.DataAccess.Define
{
    public class Public
    {
        /// <summary>
        /// 返回webResult的错误提示
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string WebResult2Str(webResult result)
        {
            if (null == result) return string.Empty;
            return string.Format("{0}({1})", result.errorMessage, result.errorCode);
        }
        /// <summary>
        /// 返回ClassedException的错误提示
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ClassedException2Str(ClassedException exp)
        {
            if (null == exp) return string.Empty;
            return string.Format("{0}({1})", exp.Message, exp.ErrorCode);
        }
        /// <summary>
        /// 返回WebException的错误提示
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string WebException2Str(webExchange result)
        {
            if (null == result) return string.Empty;
            return string.Format("{0}({1})", result.errorMessage, result.errorCode);
        }
        /// <summary>
        /// 返回WebException的错误提示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string WebException2Str<T>(webExchange<T> result)
        {
            if (null == result) return string.Empty;
            return string.Format("{0}({1})", result.errorMessage, result.errorCode);
        }
        /// <summary>
        /// 返回固定精度数值，无法转换则返回0.0M
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object ret)
        {
            if (IsNullOrDBNull(ret)) return 0.0m;
            try
            {
                return Convert.ToDecimal(ret);
            }
            catch
            {
                return 0.0m;
            }
        }
        /// <summary>
        /// 对象深度复制,对象要标示为可序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ObjectClone<T>(T obj) where T : class
        {
            byte[] tmpObj = ObjectToByteArray.ObjectToBytes(obj);
            if (null != tmpObj && 0 < tmpObj.Length)
            {
                T result = ByteArrayToObject.BytesToObject<T>(tmpObj);
                tmpObj = null;
                return result;
            }
            return null;
        }
        /// <summary>
        /// 把身份证号剖解成年月日
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <param name="YYYY"></param>
        /// <param name="MM"></param>
        /// <param name="DD"></param>
        public static void SplitIdCardToYYMMDD(string idCardNo, out int YYYY, out int MM, out int DD)
        {
            string strY = idCardNo.Substring(6, 4);
            string strM = idCardNo.Substring(10, 2);
            string strD = idCardNo.Substring(12, 2);
            YYYY = BIN.DataAccess.Define.Public.GetIntValue(strY);
            MM = BIN.DataAccess.Define.Public.GetIntValue(strM);
            DD = BIN.DataAccess.Define.Public.GetIntValue(strD);
        }
        /// <summary>
        /// 转换成日期
        /// </summary>
        /// <param name="YYYY"></param>
        /// <param name="MM"></param>
        /// <param name="DD"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(int YYYY, int MM, int DD)
        {
            try
            {
                return new DateTime(YYYY, MM, DD);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 生成基础资料ID
        /// </summary>
        /// <param name="uctTimeNow"></param>
        /// <param name="suffix"></param>
        /// <param name="fixLen">固定长度</param>
        /// <param name="fillChar">如果没有到达指定的长度是否使用字符填充('\0'使用随机数填充)</param>
        /// <returns></returns>
        public static string MakeBaseDataId(DateTime uctTimeNow, string suffix, int fixLen, char fillChar)
        {
            string ret = string.Format("{0}{1}", uctTimeNow.Ticks.ToString(), suffix);
            int len = ret.Length;
            if (len == fixLen) //刚好等于指定的长度
                return ret;
            else if (len > fixLen) //大于指定的长度
                return ret.Substring(0, fixLen);
            else
            {
                return string.Format("{0}{1}", ret,
                    (fillChar != '\0')?
                    fillChar.ToString().PadRight(fixLen - len, fillChar)
                    :  AnyRadixConvert.GetRandomNum(fixLen - len));
            }
        }
        /// <summary>
        /// 组合后缀生成唯一ID
        /// </summary>
        /// <param name="UctTimeNow"></param>
        /// <param name="suffix">后缀只能是由26个字母加数字组成</param>
        /// <param name="SpecifiedLen">指定的长度,最小19位</param>
        /// <param name="fillChar">如果没有到达指定的长度是否使用字符填充('\0'使用随机数填充)</param>
        /// <returns></returns>
        public static string MakeTradeNo(DateTime UctTimeNow, string suffix, int fixLen, char fillChar = '\0')
        {
            if (fixLen < 19) fixLen = 19;
            string ret = UctTimeNow.ToString("yyMMddHHmmssfffff");
            string suffixStr = string.Empty;
            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = RegExMatch.RegexReplace(suffix, @"[^A-Za-z0-9]+", "", true);
                try
                {
                    suffixStr = AnyRadixConvert.X2X(suffix, 36, 10);
                }
                catch
                {
                    suffixStr = string.Empty;
                }
            }
            ret += suffixStr;
            int len = ret.Length;
            if (len == fixLen) //刚好等于指定的长度
                return ret;
            else if (len > fixLen) //大于指定的长度
                return ret.Substring(0, fixLen);
            else
            {
                return string.Format("{0}{1}", ret,(fillChar != '\0')?
                    fillChar.ToString().PadRight(fixLen - len, fillChar):
                     AnyRadixConvert.GetRandomNum(fixLen - len));
            }
        }
        public static int Bool2Int(bool val)
        {
            return (val ? 1 : 0);
        }
        public static byte Bool2byte(bool val)
        {
            return (byte)(val ? 1 : 0);
        }
        public static Int16 Bool2Int16(bool val)
        {
            return (Int16)(val ? 1 : 0);
        }
        public static string[,] ToString2Arr(List<clsKeyValue<string, string>> strLst)
        {
            if (null == strLst || 0 == strLst.Count) return null;
            int count=strLst.Count ;
            string[,] ret = new string[count, 2];
            int i = 0;
            foreach (clsKeyValue<string, string> ele in strLst)
            {
                ret[i, 0] = ele.Key;
                ret[i, 1] = ele.Value;
                ++i;
            }
            return ret;
        }
        /// <summary>
        /// 判断类是否实现某接口
        /// </summary>
        /// <param name="ClsName"></param>
        /// <param name="IntfName"></param>
        /// <returns></returns>
        public static bool ClassInteritFromInterface(Type ClsType, Type IntfType)
        {
            if (!IntfType.IsInterface) return false;
            Type ret = ClsType.GetInterface(IntfType.ToString());
            return (null!=ret && ret.Equals(IntfType));
        }
        public static void ShowException(Exception ex, string Caption)
        {
            System.Windows.Forms.MessageBox.Show(ex.Message,  Caption,
                System.Windows.Forms.MessageBoxButtons.OK, 
                System.Windows.Forms.MessageBoxIcon.Error);
        }
        public static void ShowException2(Exception ex, string Caption)
        {
            string Ret = string.Format("Message:{0} {1} Source:{2} {1} StackTrace:{3}",
                ex.Message, Environment.NewLine,
                ex.Source, ex.StackTrace);
            Public.ShowMessage(MSG_TYPE.ERROR, Ret, Caption);
        }
        public static void ShowMessage(MSG_TYPE type, string Content, string Caption)
        {
            System.Windows.Forms.MessageBoxIcon icon = System.Windows.Forms.MessageBoxIcon.Information;
            if (type == MSG_TYPE.ALERT)
                icon = System.Windows.Forms.MessageBoxIcon.Exclamation;
            else if (type == MSG_TYPE.ERROR)
                icon = System.Windows.Forms.MessageBoxIcon.Error;
            else
                icon = System.Windows.Forms.MessageBoxIcon.Information;
            System.Windows.Forms.MessageBox.Show(Content, Caption,
                  System.Windows.Forms.MessageBoxButtons.OK, icon);
        }
        public static void ShowMessage(string Content, string Caption)
        {
            System.Windows.Forms.MessageBox.Show(Content,
                 Caption,
                  System.Windows.Forms.MessageBoxButtons.OK,
                  System.Windows.Forms.MessageBoxIcon.Information);
        }
        public static bool GetMessageBoxYesNoResult(string Question, string Caption)
        {
            return (
                System.Windows.Forms.MessageBox.Show(Question, Caption,
                         System.Windows.Forms.MessageBoxButtons.YesNo,
                          System.Windows.Forms.MessageBoxIcon.Question,
                          System.Windows.Forms.MessageBoxDefaultButton.Button2)
                          == System.Windows.Forms.DialogResult.Yes);
        }
        public static string GetSaveFileName(string Filter)
        {
            System.Windows.Forms.SaveFileDialog frm = null;
            try
            {
                frm = new System.Windows.Forms.SaveFileDialog();
                frm.Filter = Filter;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return (frm.FileName);
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (frm != null) frm.Dispose();
            }
        }
        public static string GetSaveFileName(string Filter, string InitDir)
        {
            System.Windows.Forms.SaveFileDialog frm = null;
            try
            {
                frm = new System.Windows.Forms.SaveFileDialog();
                frm.Filter = Filter;
                frm.InitialDirectory = InitDir;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return (frm.FileName);
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (frm != null) frm.Dispose();
            }
        }
        public static string GetSaveFileName(string Filter, string InitDir, string InitFileName)
        {
            System.Windows.Forms.SaveFileDialog frm = null;
            try
            {
                frm = new System.Windows.Forms.SaveFileDialog();
                frm.Filter = Filter;
                if (InitDir != "") frm.InitialDirectory = InitDir;
                if (InitFileName != "") frm.FileName = InitFileName;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return (frm.FileName);
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (frm != null) frm.Dispose();
            }
        }
        public static string GetShortFileName(string FileName)
        {
            int Index = FileName.LastIndexOf('\\');
            if (Index < 0) return FileName;
            return FileName.Substring(Index + 1, FileName.Length - Index - 1);
        }
        public static string GetOpenFile(string filter, string Dir, string fileName)
        {
            System.Windows.Forms.OpenFileDialog diag = null;
            try
            {
                diag = new System.Windows.Forms.OpenFileDialog();
                diag.Filter = filter;
                diag.InitialDirectory = Dir;
                if (!"".Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    diag.FileName=fileName;
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return diag.FileName;
                }
                else
                    return string.Empty;
            }
            finally
            {
                if (null != diag) diag.Dispose();
            }
        }
        public static void ExecuteException(Action method)
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Public.ShowException(ex, "Error");
            }
        }
        public static void ExecuteException(Action method,string Caption)
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Public.ShowException(ex, Caption);
            }
        }
        public static string GetApplicationBaseDir()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!appDir.EndsWith("\\")) appDir += "\\";
            return appDir;
        }
        public static int BeginProcess(string ProgName, string Args, string WorkDir)
        {
            Process p = new Process();
            try
            {
                p.StartInfo.FileName = ProgName;
                if (WorkDir != "") p.StartInfo.WorkingDirectory = WorkDir;
                if ("" != Args) p.StartInfo.Arguments = Args;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                return p.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool IsProgExists(int ProgId)
        {
            return (null != Process.GetProcessById(ProgId));
        }
        public static bool KillProg(int ProgId)
        {
           Process proc= Process.GetProcessById(ProgId);
           if (null != proc)
           {
               proc.Kill();
               return true;
           }
           return false;
        }
        private static string _appDir = string.Empty;
        public static string AppDir
        {
            get
            {
                if (string.Empty.Equals(_appDir))
                {
                    _appDir = GetApplicationBaseDir();
                }
                return _appDir;
            }
        }
        public const string GuidZero = "00000000-0000-0000-0000-000000000000";
        public static bool IsObjectNull(object obj)
        {
            return (obj == null || obj == DBNull.Value);
        }
        public static object ObjectToGuidObj(object obj)
        {
            if (IsObjectNull(obj))
                return DBNull.Value;
            else
                return GetObjGUID(obj);
        }
        public static string GetObjectString(object obj)
        {
            return ((obj == null || obj == DBNull.Value || obj.ToString() == "") ? "" : obj.ToString());
        }
        public static string GetObjectString(object obj, string EmptyIf)
        {
            string ret = GetObjectString(obj);
            if (string.Empty.Equals(ret))
                return EmptyIf;
            else
                return ret;
        }
        public static string GetObjStrUpperTrim(object obj)
        {
            return GetObjectString(obj).Trim().ToUpper();
        }
        public static string GetObjStrUpperTrim(object obj, string EmptyIf)
        {
            string ret = GetObjStrUpperTrim(obj);
            if (string.Empty.Equals(ret))
                return EmptyIf;
            else
                return ret;
        }
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Half2Full(string input)
        {
            char[] cc = input.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] == 32)
                {
                    cc[i] = (char)12288;
                    continue;
                }
                if (cc[i] < 127 && cc[i] > 32)
                {
                    cc[i] = (char)(cc[i] + 65248);
                }
            }
            return new string(cc);
        }
        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Full2Half(string input)
        {
            char[] cc = input.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] == 12288)
                {
                    cc[i] = (char)32;
                    continue;
                }
                if (cc[i] > 65280 && cc[i] < 65375)
                {
                    cc[i] = (char)(cc[i] - 65248);
                }

            }
            return new string(cc);
        }
#if !DOTNET35
        public static Guid GetObjGUID(object obj)
        {
            string txt = GetObjectString(obj);
            Guid Ret;
            if (!(Guid.TryParse(txt, out Ret))) Ret = Guid.Parse(GuidZero);
            return Ret;
        }
#else
        public static Guid GetObjGUID(object obj)
        {
            string txt = GetObjectString(obj);
            Guid Ret;
            try
            {
                Ret = new Guid(txt);
            }
            catch
            {
                Ret = Guid.Empty;
            }
            return Ret;
        }
#endif
        public static object DBNullIf(object obj)
        {
            if (null == obj || DBNull.Value == obj) return DBNull.Value;
            return obj;
        }
        public static bool IsNullOrDBNull(object obj)
        {
            return (null == obj || DBNull.Value == obj);
        }
#if !DOTNET35
        public static bool IsGuid(string Str, out Guid Ret)
        {
            Guid.TryParse(GuidZero, out Ret);
            return (Guid.TryParse(Str, out Ret));
        }
        public static string CombineDir(params string[] Para)
        {
            return System.IO.Path.Combine(Para);
        }
        public static bool IsGuid(string Str)
        {
            Guid ID = Guid.NewGuid();
            bool Ret = Guid.TryParse(Str, out ID);
            return (Ret);
        }
#else
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsStrNullOrEmpty(string str)
        {
            return (null == str || 0 == str.Length);
        }
        public static bool IsGuid(string Str, out Guid Ret)
        {
            Ret = GetObjGUID(Str);
            return (!Ret.Equals(Guid.Empty));
        }
        public static string CombineDir(string Para1,string Para2)
        {
            return System.IO.Path.Combine(Para1,Para2);
        }
        public static bool IsGuid(string Str)
        {
            Guid Ret = GetObjGUID(Str);
            return (!Ret.Equals(Guid.Empty));
        }
#endif
        public static bool WriteFile(string fileName, byte[] data,bool Append)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName,
                Append? System.IO.FileMode.Append: System.IO.FileMode.CreateNew
                , System.IO.FileAccess.Write))
            {
                using (System.IO.BinaryWriter rw = new System.IO.BinaryWriter(fs))
                {
                    rw.Write(data);
                    rw.Close();
                }
                fs.Close();
            }
            return FileExists(fileName);
        }
        public static bool FileExists(string FileName)
        {
            return (System.IO.File.Exists(FileName));
        }
        public static string ReadTextFile(string FileName)
        {
            if (!(FileExists(FileName)))
                throw new Exception("No files exists!");
            System.IO.StreamReader sr = null;
            try
            {
                sr = new System.IO.StreamReader(FileName, System.Text.Encoding.UTF8);
                return sr.ReadToEnd();
            }
            finally
            {
                if (null != sr)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }
        public static void WriteTextFile(string FileName, string ConTent)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(FileName, false);
                sw.Write(ConTent);
            }
            finally
            {
                if (null != sw)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        public static void WriteTextFile(string FileName, string ConTent, bool Append)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(FileName, Append);
                sw.Write(ConTent);
            }
            finally
            {
                if (null != sw)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        public static void WriteTextFileLine(string FileName, string ConTent, bool Append)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(FileName);
                if (fi.Exists && fi.Length / (1024.0f * 1024) > 1.0)
                    sw = new System.IO.StreamWriter(FileName, false);//删除以前的内容
                else
                    sw = new System.IO.StreamWriter(FileName, Append);
                sw.WriteLine(ConTent);
            }
            finally
            {
                if (null != sw)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        public static void CheckFolderExists(string Folder)
        {
            if (!(System.IO.Directory.Exists(Folder))) System.IO.Directory.CreateDirectory(Folder);
        }
        public static string GetParentDir(string Dir)
        {
            string Ret = Dir;
            if (!Ret.EndsWith("\\")) Ret += "\\";
            Ret += "..\\";
            return System.IO.Path.GetFullPath(Ret);
        }
        public static string GetParentDir2(string dir)
        {
            return new System.IO.DirectoryInfo(dir).Parent.FullName;
        }
        public static void CheckFolderExistsViaFileName(string FileName)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(FileName);
            if (null != fi) CheckFolderExists(fi.DirectoryName);
        }
        /// <summary>
        /// dt2比dt1大多少
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static double DateDiff(DATEPART dp, DateTime dt1, DateTime dt2)
        {
            TimeSpan t1 = new TimeSpan(dt1.Ticks);
            TimeSpan t2 = new TimeSpan(dt2.Ticks);
            TimeSpan Diff = t1.Subtract(t2).Duration();
            double Ret = 0;
            switch (dp)
            {
                case DATEPART.DAY:
                    Ret = (Diff.TotalDays);
                    break;
                case DATEPART.HOUR:
                    Ret = (Diff.TotalHours);
                    break;
                case DATEPART.MINUTE:
                    Ret = (Diff.TotalMinutes);
                    break;
                case DATEPART.SECOND:
                    Ret = (Diff.TotalSeconds);
                    break;
                default:
                    Ret = 0.0;
                    break;
            }
            bool Little = (dt2 < dt1);
            //return Ret;
            return (Little ? Ret * -1 : Ret);
        }
        public static double DateDiff(string howtocompare, System.DateTime startDate, System.DateTime endDate)
        {
            double diff = 0;
            try
            {
                System.TimeSpan TS = new System.TimeSpan(startDate.Ticks - endDate.Ticks);
                switch (howtocompare)
                {
                    case "m ":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s ":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t ":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm ":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy ":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "MM":
                        diff = Convert.ToDouble(TS.TotalDays / 365 * 12.0);
                        break;
                    case "q ":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    default:
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
            }
            catch
            {
                diff = -1;
                throw;
            }
            return diff;
        }
        public static COMPARE_RESULT DateCompare(DateTime Date1, DateTime Date2)
        {
            int value = Date1.CompareTo(Date2);
            return GetEnumItemByValue<COMPARE_RESULT>(value);
        }
        public static T GetEnumItemByValue<T>(int Value)
        {
            if (!(typeof(T).IsEnum)) throw new Exception("Not enum !");
            return (T)Enum.ToObject(typeof(T), Value);
        }
        public static T GetEnumByKey<T>(string Key)
        {
            if (!(typeof(T).IsEnum)) throw new Exception("Not enum!");
            return (T)Enum.Parse(typeof(T), Key);
        }
        public static bool TryParseInt(object obj, out int Ret)
        {
            string value = GetObjectString(obj);
            return Int32.TryParse(value, out Ret);
        }
        public static int GetIntValue(object Val)
        {
            if (DBNull.Value == Val || null == Val) return 0;
            int Ret = 0;
            if (!(int.TryParse(Val.ToString(), out Ret))) return 0;
            return Ret;
        }
        private static string _certFileName = string.Empty;
        private static string CertFileName
        {
            get
            {
                if (ShareMet.StrAreSameUpperTrim(string.Empty, _certFileName))
                {
                    _certFileName = CombineDir(Public.AppDir, @"Certification\\Client.cert");
                }
                return _certFileName;
            }
        }
        private static structClientCertSetting _clientCert = null;
        public static structClientCertSetting ClientCertInfo
        {
            get
            {
                if (null == _clientCert)
                { 
                 _clientCert=clsClientCertInfo.GetCertInfo(CertFileName);
                }
                return _clientCert;
            }
        }
        private static readonly Dictionary<string, structClientCertSetting>
            _savedCertSetting = new Dictionary<string, structClientCertSetting>();
        private static readonly object _lockObj = new object();
        public static structClientCertSetting GetClientCertInfo(string clientCertFileName)
        {
            structClientCertSetting ret=null;
            if (_savedCertSetting.TryGetValue(clientCertFileName, out ret)) return ret;
            string fileName = ShareMet.CombineDir(ShareMet.GetApplicationBaseDir(),
                string.Format(@"Certification\{0}", clientCertFileName));
            if (!ShareMet.FileExists(fileName)) return null;
            ret=clsClientCertInfo.GetCertInfo(fileName );
            if (null != ret)
            {
                lock (_lockObj)
                {
                    _savedCertSetting.Add(clientCertFileName, ret);
                }
            }
            return ret ;
        }
        private static readonly Dictionary<string, X509Certificate2>
            _savedX509Cert = new Dictionary<string, X509Certificate2>();
        private static readonly object _lockObj2 = new object();
        public static X509Certificate2 GetX509Certificate2(string clientCertShortName)
        {
            X509Certificate2 ret = null;
            if (_savedX509Cert.TryGetValue(clientCertShortName, out ret)) return ret;
            structClientCertSetting cltCertInfo = GetClientCertInfo(clientCertShortName);
            if (null == cltCertInfo) return null;
            if (cltCertInfo.FindViaCertFile)
            {
                string fileName = cltCertInfo.CertFileName;
                if (!Public.FileExists(fileName))
                {
                    fileName = Public.CombineDir(
                        Public.CombineDir(Public.AppDir, "Certification"), fileName);
                }
                if (!Public.FileExists(fileName))
                    throw new Exception(string.Format("Cert File '{0}' doesn't exist!", cltCertInfo.CertFileName));
                ret = new X509Certificate2(fileName);
            }
            else
            {
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
                X509Certificate2Collection fcollection = (X509Certificate2Collection)store.Certificates;
                string certName = string.Format("CN={0}", ClientCertInfo.CertName);
                foreach (X509Certificate2 ele in fcollection)
                {
                    if (ele.Subject.Equals(certName, StringComparison.OrdinalIgnoreCase))
                    {
                        ret = ele;
                        break;
                    }
                }
            }
            if (null != ret)
            {
                lock (_lockObj2)
                {
                    _savedX509Cert.Add(clientCertShortName, ret);
                }
            }
            return ret;
        }
        private static X509Certificate2 _x509Cert = null;
        public static X509Certificate2 X509Cert
        {
            get
            {
                if (null == _x509Cert)
                {
                    if (ClientCertInfo.FindViaCertFile)
                    {
                        if (!Public.FileExists(ClientCertInfo.CertFileName))
                            throw new Exception(string.Format("Cert File '{0}' doesn't exist!", ClientCertInfo.CertFileName));
                        _x509Cert = new X509Certificate2(ClientCertInfo.CertFileName);
                    }
                    else
                    {
                        X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                        store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
                        X509Certificate2Collection fcollection = (X509Certificate2Collection)store.Certificates;
                        string certName = string.Format("CN={0}", ClientCertInfo.CertName);
                        foreach (X509Certificate2 ele in fcollection)
                        {
                            if (ele.Subject.Equals(certName, StringComparison.OrdinalIgnoreCase))
                            {
                                _x509Cert = ele;
                                break;
                            }
                        }
                    }
                }
                return _x509Cert;
            }
        }
        /// <summary>
        /// 获取证书WCF的代理实例
        /// clientCertFile为BIN定义的cert文件(不是证书文件,是证书定义文件)如Certification\Client.cert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Url"></param>
        /// <param name="certName"></param>
        /// <returns></returns>
        public static T GetWSHttpCertClient<T>(string Url, string clientCertShortFile)
        {
            structClientCertSetting certInfo = GetClientCertInfo(clientCertShortFile);
            if (null == certInfo)
                throw new ArgumentException(clientCertShortFile + " doesn't exist!");
            if (-1 == certInfo.ExpiredDate.CompareTo(DateTime.Today))
                throw new Exception("Certification user was expired!");
            string cryptedId = BIN.DataAccess.Encrypt.EncryptString(certInfo.Id);
            string cryptedPwd = BIN.DataAccess.Encrypt.EncryptString(certInfo.Password);
            X509Certificate2 cert = GetX509Certificate2(clientCertShortFile);
            if (null == cert) throw new Exception("Unable to get client certification!");
            EndpointIdentity iden = EndpointIdentity.CreateX509CertificateIdentity(cert);
            EndpointAddress addr = new EndpointAddress(new Uri(Url), iden);
            ChannelFactory<T> channel = new ChannelFactory<T>(staticBinding.CertWSHttpBinding, addr);
            channel.Credentials.ServiceCertificate.DefaultCertificate = cert;
            channel.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            channel.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = null;
            channel.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.Online;
            channel.Credentials.ServiceCertificate.Authentication.TrustedStoreLocation = StoreLocation.CurrentUser;
            channel.Credentials.UserName.UserName = cryptedId;
            channel.Credentials.UserName.Password = cryptedPwd;
            return channel.CreateChannel();
        }
        /// <summary>
        /// 泛型为接口,
        /// 注意此方法未必能调用成功,要看WCF的代理类是怎样生成的,OK的如SQLMetaProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Url"></param>
        /// <param name="MethodName"></param>
        /// <param name="Para"></param>
        /// <returns></returns>
        public static object WcfInvokeWSHttpCert<T>(
            string Url, string MethodName, params object[] Para) 
        {
            structClientCertSetting certInfo = ClientCertInfo;
            if (null == certInfo) throw new Exception("Certification info doesn't exist!");
            if (-1 == certInfo.ExpiredDate.CompareTo(DateTime.Today))
                throw new Exception("Certification user was expired!");
            string cryptedId = BIN.DataAccess.Encrypt.EncryptString(certInfo.Id);
            string cryptedPwd = BIN.DataAccess.Encrypt.EncryptString(certInfo.Password);
            X509Certificate2 cert = X509Cert;
            if (null == cert) throw new Exception("Unable to get client certification!");
            EndpointIdentity iden = EndpointIdentity.CreateX509CertificateIdentity(cert);
            EndpointAddress addr = new EndpointAddress(new Uri(Url),iden);
            using (ChannelFactory<T> channel = new ChannelFactory<T>(staticBinding.CertWSHttpBinding, addr))
            {
                channel.Credentials.ServiceCertificate.DefaultCertificate = cert;
                channel.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                channel.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = null;
                channel.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.Online;
                channel.Credentials.ServiceCertificate.Authentication.TrustedStoreLocation = StoreLocation.CurrentUser;
                channel.Credentials.UserName.UserName = cryptedId;
                channel.Credentials.UserName.Password = cryptedPwd;
                T instance = channel.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(MethodName);
                        if (null != Para && 0 < Para.Length)
                            return mi.Invoke(instance, Para);
                        else
                            return mi.Invoke(instance,null);
                    }
                    catch (TimeoutException ex)
                    {
                        throw ex;
                    }
                    catch (CommunicationException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ee)
                    {
                        throw ee;
                    }
                }
            }
        }
        /// <summary>
        /// 泛型为接口,
        /// 注意此方法未必能调用成功,要看WCF的代理类是怎样生成的,OK的如SQLMetaProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BindType"></param>
        /// <param name="Url"></param>
        /// <param name="MethodName"></param>
        /// <param name="Para"></param>
        /// <returns></returns>
        public static object WcfInvoke<T>(BindingType BindType,string Url,string MethodName,params object[] Para)
        {
            Binding bind = null;
            if (BindType.Equals(BindingType.BASICHTTP))
                bind = staticBinding.BasicHttpBinding;
            else if (BindType.Equals(BindingType.TCP))
                bind = staticBinding.TcpBinding;
            else if (BindType.Equals(BindingType.STREAM))
                bind = staticBinding.StreamBinding;
            else if (BindType.Equals(BindingType.WSHTTP))
                bind = staticBinding.WshttpBinding;
            else if (BindType.Equals(BindingType.GZIPHTTP))
                bind = staticBinding.GZipHttpBinding;
            else if (BindType.Equals(BindingType.GZIPTCP))
                bind = staticBinding.GZipTcpBinding;
            else if (BindType.Equals(BindingType.CERTWSHTTP))
                bind = staticBinding.CertWSHttpBinding;
            else if (BindType.Equals(BindingType.CERTHTTP))
                bind = staticBinding.CertBasicHttpBinding;
            else
                throw new Exception("No binding found!");
            EndpointAddress Addr = new EndpointAddress(Url);
            using (ChannelFactory<T> channel = new ChannelFactory<T>(bind, Addr))
            {
                T instance = channel.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(MethodName);
                        return mi.Invoke(instance, Para);
                    }
                    catch (TimeoutException ex)
                    {
                        throw ex;
                    }
                    catch (CommunicationException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ee)
                    {
                        throw ee;
                    }
                }
            }
        }
        public static Type GetClassType(string FileName, string FullClassName)
        {
            if (!(FileExists(FileName))) return null;
            Assembly Ass = Assembly.LoadFile(FileName);
            return Ass.GetType(FullClassName, true, true);
        }
        /// <summary>
        /// 动态类反射
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Class"></param>
        /// <returns></returns>
        public static object ReflectionMakeObj(string FileName, string Class)
        {
            if (!(FileExists(FileName))) return null;
            Assembly Ass = Assembly.LoadFile(FileName);
            Type myType = Ass.GetType(Class, true, true);
            return (Activator.CreateInstance(myType));
        }
        /// <summary>
        /// 动态类反射
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Class"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static object ReflectionMakeObj(string FileName, string Class, object[] Args)
        {
            if (!(FileExists(FileName))) return null;
            Assembly Ass = Assembly.LoadFile(FileName);
            Type myType = Ass.GetType(Class, true, true);
            return (Activator.CreateInstance(myType, Args));
        }
        /// <summary>
        /// 反射调用方法
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="className"></param>
        /// <param name="Method"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static object ReflectionInvokeMethods(string fileName, string className, string Method, object[] Args)
        {
            if (!(FileExists(fileName))) return null;
            Assembly Ass = Assembly.LoadFile(fileName);
            Type myType = Ass.GetType(className, true, true);
            return myType.InvokeMember(Method, 
                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static,
                null, null, Args);
        }
    }
}
