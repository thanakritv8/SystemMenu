using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Configuration;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyCompany.Data;
using System.Web.Routing;
using System.Drawing;
using System.Drawing.Imaging;
using MyCompany.Handlers;
using MyCompany.Web;

namespace MyCompany.Services
{
	public abstract class ServiceRequestHandler
    {
        
        public abstract object HandleRequest(DataControllerService service, JObject args);
        
        public virtual object HandleException(JObject args, Exception ex)
        {
            while (ex.InnerException != null)
            	ex = ex.InnerException;
            ServiceRequestError error = new ServiceRequestError();
            error.Message = ex.Message;
            error.ExceptionType = ex.GetType().ToString();
            error.StackTrace = ex.StackTrace;
            return error;
        }
    }
    
    public class GetPageServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            PageRequest r = args["request"].ToObject<PageRequest>();
            return service.GetPage(((string)(args["controller"])), ((string)(args["view"])), r);
        }
    }
    
    public class GetPageListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetPageList(args["requests"].ToObject<PageRequest[]>());
        }
    }
    
    public class GetListOfValuesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            DistinctValueRequest r = args["request"].ToObject<DistinctValueRequest>();
            return service.GetListOfValues(((string)(args["controller"])), ((string)(args["view"])), r);
        }
    }
    
    public class ExecuteServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            ActionArgs a = args["args"].ToObject<ActionArgs>();
            return service.Execute(((string)(args["controller"])), ((string)(args["view"])), a);
        }
    }
    
    public class ExecuteListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.ExecuteList(args["requests"].ToObject<ActionArgs[]>());
        }
    }
    
    public class GetCompletionListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetCompletionList(((string)(args["prefixText"])), ((int)(args["count"])), ((string)(args["contextKey"])));
        }
    }
    
    public class LoginServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Login(((string)(args["username"])), ((string)(args["password"])), ((bool)(args["createPersistentCookie"])));
        }
    }
    
    public class LogoutServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            service.Logout();
            return null;
        }
    }
    
    public class RolesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Roles();
        }
    }
    
    public class ThemesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Themes();
        }
    }
    
    public class SavePermalinkServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            service.SavePermalink(((string)(args["link"])), ((string)(args["html"])));
            return null;
        }
    }
    
    public class EncodePermalinkServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.EncodePermalink(((string)(args["link"])), ((bool)(args["rooted"])));
        }
    }
    
    public class ListAllPermalinksServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.ListAllPermalinks();
        }
    }
    
    public class GetSurveyServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetSurvey(((string)(args["name"])));
        }
    }
    
    public class ServiceRequestError
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _exceptionType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _message;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _stackTrace;
        
        public string ExceptionType
        {
            get
            {
                return this._exceptionType;
            }
            set
            {
                this._exceptionType = value;
            }
        }
        
        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }
        
        public string StackTrace
        {
            get
            {
                return this._stackTrace;
            }
            set
            {
                this._stackTrace = value;
            }
        }
    }
    
    public class WorkflowResources
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, string> _staticResources;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<Regex> _dynamicResources;
        
        public WorkflowResources()
        {
            _staticResources = new SortedDictionary<string, string>();
            _dynamicResources = new List<Regex>();
        }
        
        public SortedDictionary<string, string> StaticResources
        {
            get
            {
                return this._staticResources;
            }
            set
            {
                this._staticResources = value;
            }
        }
        
        public List<Regex> DynamicResources
        {
            get
            {
                return this._dynamicResources;
            }
            set
            {
                this._dynamicResources = value;
            }
        }
    }
    
    public partial class WorkflowRegister : WorkflowRegisterBase
    {
    }
    
    public class WorkflowRegisterBase
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, WorkflowResources> _resources;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, List<string>> _roleRegister;
        
        public WorkflowRegisterBase()
        {
            // initialize system workflows
            _resources = new SortedDictionary<string, WorkflowResources>();
            RegisterBuiltinWorkflowResources();
            foreach (SiteContentFile w in ApplicationServices.Current.ReadSiteContent("sys/workflows%", "%"))
            {
                string text = w.Text;
                if (!(String.IsNullOrEmpty(text)))
                {
                    WorkflowResources wr = null;
                    if (!(Resources.TryGetValue(w.PhysicalName, out wr)))
                    {
                        wr = new WorkflowResources();
                        Resources[w.PhysicalName] = wr;
                    }
                    foreach (string s in text.Split(new char[] {
                                '\n'}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string query = s.Trim();
                        if (!(String.IsNullOrEmpty(query)))
                        	if (s.StartsWith("regex "))
                            {
                                string regexQuery = s.Substring(6).Trim();
                                if (!(String.IsNullOrEmpty(regexQuery)))
                                	try
                                    {
                                        wr.DynamicResources.Add(new Regex(regexQuery, RegexOptions.IgnoreCase));
                                    }
                                    catch (Exception )
                                    {
                                    }
                            }
                            else
                            	wr.StaticResources[query.ToLower()] = query;
                    }
                }
            }
            // read "role" workflows from the register
            _roleRegister = new SortedDictionary<string, List<string>>();
            foreach (SiteContentFile rr in ApplicationServices.Current.ReadSiteContent("sys/register/roles%", "%"))
            {
                string text = rr.Text;
                if (!(String.IsNullOrEmpty(text)))
                {
                    List<string> workflows = null;
                    if (!(RoleRegister.TryGetValue(rr.PhysicalName, out workflows)))
                    {
                        workflows = new List<string>();
                        RoleRegister[rr.PhysicalName] = workflows;
                    }
                    foreach (string s in text.Split(new char[] {
                                '\n',
                                ','}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string name = s.Trim();
                        if (!(String.IsNullOrEmpty(name)))
                        	workflows.Add(name);
                    }
                }
            }
        }
        
        public SortedDictionary<string, WorkflowResources> Resources
        {
            get
            {
                return this._resources;
            }
            set
            {
                this._resources = value;
            }
        }
        
        public SortedDictionary<string, List<string>> RoleRegister
        {
            get
            {
                return this._roleRegister;
            }
            set
            {
                this._roleRegister = value;
            }
        }
        
        public List<string> UserWorkflows
        {
            get
            {
                List<string> workflows = ((List<string>)(HttpContext.Current.Items["WorkflowRegister_UserWorkflows"]));
                if (workflows == null)
                {
                    workflows = new List<string>();
                    IIdentity identity = HttpContext.Current.User.Identity;
                    if (identity.IsAuthenticated)
                    	foreach (SiteContentFile urf in ApplicationServices.Current.ReadSiteContent("sys/register/users%", identity.Name))
                        {
                            string text = urf.Text;
                            if (!(String.IsNullOrEmpty(text)))
                            	foreach (string s in text.Split(new char[] {
                                            '\n',
                                            ','}, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string name = s.Trim();
                                    if (!(String.IsNullOrEmpty(name)) && !(workflows.Contains(name)))
                                    	workflows.Add(name);
                                }
                        }
                    // enumerate role workflows
                    bool isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
                    foreach (string role in RoleRegister.Keys)
                    	if ((((role == "?") && !(isAuthenticated)) || ((role == "*") && isAuthenticated)) || DataControllerBase.UserIsInRole(role))
                        	foreach (string name in RoleRegister[role])
                            	if (!(workflows.Contains(name)))
                                	workflows.Add(name);
                    HttpContext.Current.Items["WorkflowRegister_UserWorkflows"] = workflows;
                }
                return workflows;
            }
        }
        
        public bool Enabled
        {
            get
            {
                return (_resources.Count > 0);
            }
        }
        
        public static bool IsEnabled
        {
            get
            {
                if (!(ApplicationServices.IsSiteContentEnabled))
                	return false;
                WorkflowRegister wr = WorkflowRegister.GetCurrent();
                return ((wr != null) && wr.Enabled);
            }
        }
        
        public virtual int CacheDuration
        {
            get
            {
                return 30;
            }
        }
        
        protected virtual void RegisterBuiltinWorkflowResources()
        {
        }
        
        public static bool Allows(string fileName)
        {
            if (!(ApplicationServices.IsSiteContentEnabled))
            	return false;
            WorkflowRegister wr = WorkflowRegister.GetCurrent(fileName);
            if ((wr == null) || !(wr.Enabled))
            	return false;
            return wr.IsMatch(fileName);
        }
        
        public bool IsMatch(string physicalPath, string physicalName)
        {
            string fileName = physicalPath;
            if (String.IsNullOrEmpty(fileName))
            	fileName = physicalName;
            else
            	fileName = ((fileName + "/") 
                            + physicalName);
            return IsMatch(fileName);
        }
        
        public bool IsMatch(string fileName)
        {
            fileName = fileName.ToLower();
            List<string> activeWorkflows = UserWorkflows;
            foreach (string workflow in activeWorkflows)
            {
                WorkflowResources resourceList = null;
                if (Resources.TryGetValue(workflow, out resourceList))
                {
                    if (resourceList.StaticResources.ContainsKey(fileName))
                    	return true;
                    foreach (Regex re in resourceList.DynamicResources)
                    	if (re.IsMatch(fileName))
                        	return true;
                }
            }
            return false;
        }
        
        public static WorkflowRegister GetCurrent()
        {
            return GetCurrent(null);
        }
        
        public static WorkflowRegister GetCurrent(string relativePath)
        {
            if ((relativePath != null) && (relativePath.StartsWith("sys/workflows") || relativePath.StartsWith("sys/register")))
            	return null;
            string key = "WorkflowRegister_Current";
            HttpContext context = HttpContext.Current;
            WorkflowRegister instance = ((WorkflowRegister)(context.Items[key]));
            if (instance == null)
            {
                instance = ((WorkflowRegister)(context.Cache[key]));
                if (instance == null)
                {
                    instance = new WorkflowRegister();
                    context.Cache.Add(key, instance, null, DateTime.Now.AddSeconds(instance.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
                }
                context.Items[key] = instance;
            }
            return instance;
        }
    }
    
    public enum SiteContentFields
    {
        
        SiteContentId,
        
        DataFileName,
        
        DataContentType,
        
        Path,
        
        Data,
        
        Roles,
        
        Users,
        
        Text,
        
        CacheProfile,
        
        RoleExceptions,
        
        UserExceptions,
        
        Schedule,
        
        ScheduleExceptions,
    }
    
    public class SiteContentFile
    {
        
        private object _id;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _name;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _path;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _contentType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private byte[] _data;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _physicalName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _error;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _schedule;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _scheduleExceptions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cacheProfile;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _cacheDuration;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private HttpCacheability _cacheLocation;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _cacheVaryByParams;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _cacheVaryByHeaders;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _cacheNoStore;
        
        public SiteContentFile()
        {
            this.CacheLocation = HttpCacheability.NoCache;
        }
        
        public object Id
        {
            get
            {
                return _id;
            }
            set
            {
                if ((value != null) && (value.GetType() == typeof(byte[])))
                	value = new Guid(((byte[])(value)));
                _id = value;
            }
        }
        
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        
        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }
        
        public string ContentType
        {
            get
            {
                return this._contentType;
            }
            set
            {
                this._contentType = value;
            }
        }
        
        public byte[] Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }
        
        public string PhysicalName
        {
            get
            {
                return this._physicalName;
            }
            set
            {
                this._physicalName = value;
            }
        }
        
        public string Error
        {
            get
            {
                return this._error;
            }
            set
            {
                this._error = value;
            }
        }
        
        public string Schedule
        {
            get
            {
                return this._schedule;
            }
            set
            {
                this._schedule = value;
            }
        }
        
        public string ScheduleExceptions
        {
            get
            {
                return this._scheduleExceptions;
            }
            set
            {
                this._scheduleExceptions = value;
            }
        }
        
        public string CacheProfile
        {
            get
            {
                return this._cacheProfile;
            }
            set
            {
                this._cacheProfile = value;
            }
        }
        
        public int CacheDuration
        {
            get
            {
                return this._cacheDuration;
            }
            set
            {
                this._cacheDuration = value;
            }
        }
        
        public HttpCacheability CacheLocation
        {
            get
            {
                return this._cacheLocation;
            }
            set
            {
                this._cacheLocation = value;
            }
        }
        
        public string[] CacheVaryByParams
        {
            get
            {
                return this._cacheVaryByParams;
            }
            set
            {
                this._cacheVaryByParams = value;
            }
        }
        
        public string[] CacheVaryByHeaders
        {
            get
            {
                return this._cacheVaryByHeaders;
            }
            set
            {
                this._cacheVaryByHeaders = value;
            }
        }
        
        public bool CacheNoStore
        {
            get
            {
                return this._cacheNoStore;
            }
            set
            {
                this._cacheNoStore = value;
            }
        }
        
        public string Text
        {
            get
            {
                if ((this.Data != null) && (!(String.IsNullOrEmpty(this.ContentType)) && this.ContentType.StartsWith("text/")))
                	return Encoding.UTF8.GetString(this.Data);
                return null;
            }
            set
            {
                if (value == null)
                	_data = null;
                else
                {
                    _data = Encoding.UTF8.GetBytes(value);
                    _contentType = "text/plain";
                }
            }
        }
        
        public bool IsText
        {
            get
            {
                return ((_contentType != null) && (_contentType.StartsWith("text/") || (_contentType == "application/javascript")));
            }
        }
        
        public override string ToString()
        {
            return String.Format("{0}/{1}", Path, Name);
        }
        
        public virtual bool Update()
        {
            ApplicationServices services = ApplicationServices.Create();
            StringBuilder sb = new StringBuilder("update ");
            sb.Append(services.GetSiteContentControllerName());
            sb.Append(" set ");
            sb.Append(services.SiteContentFieldName(SiteContentFields.Text));
            sb.Append(" = ");
            sb.Append(SqlStatement.GetParameterMarker(String.Empty));
            sb.Append("p0 where ");
            sb.Append(services.SiteContentFieldName(SiteContentFields.SiteContentId));
            sb.Append(" = ");
            sb.Append(SqlStatement.GetParameterMarker(String.Empty));
            sb.Append("p1");
            return (SqlText.ExecuteNonQuery(sb.ToString(), Text, Id) == 1);
        }
    }
    
    public class SiteContentFileList : List<SiteContentFile>
    {
    }
    
    public partial class ApplicationServices : EnterpriseApplicationServices
    {
        
        public static String HomePageUrl
        {
            get
            {
                return Create().UserHomePageUrl();
            }
        }
        
        public static void Initialize()
        {
            Create().RegisterServices();
        }
        
        public static object Login(string username, string password, bool createPersistentCookie)
        {
            return Create().AuthenticateUser(username, password, createPersistentCookie);
        }
        
        public static void Logout()
        {
            Create().UserLogout();
        }
        
        public static string[] Roles()
        {
            return Create().UserRoles();
        }
        
        public static JObject Themes()
        {
            return Create().UserThemes();
        }
    }
    
    public class ApplicationServicesBase
    {
        
        public static bool EnableMobileClient = true;
        
        public static string DesignerPort = String.Empty;
        
        private JObject _defaultSettings;
        
        private static bool _enableMinifiedCss;
        
        public static Regex MobileAgentBRegex = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino|android|ipad|playbook|silk", (RegexOptions.IgnoreCase | RegexOptions.Multiline));
        
        public static Regex MobileAgentVRegex = new Regex("1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(" +
                "av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|" +
                "be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|c" +
                "ell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p" +
                ")o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-" +
                "|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|" +
                "hi(pt|ta)|hp( i|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\\-(20|go|ma)|i23" +
                "0|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|k" +
                "ddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|\\-[a" +
                "-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(rc|ri)|mi(o8|" +
                "oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n2" +
                "0[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|i)|nzp" +
                "h|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(" +
                "ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|i\\-)|q" +
                "tek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|" +
                "sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3" +
                "|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|t" +
                "cl\\-|tdg\\-|tel(i|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|u" +
                "tst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|" +
                "80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\\-|your|zeto|" +
                "zte\\-", (RegexOptions.IgnoreCase | RegexOptions.Multiline));
        
        public static Regex NameValueListRegex = new Regex("^\\s*(?\'Name\'\\w+)\\s*=\\s*(?\'Value\'[\\S\\s]+?)\\s*$", RegexOptions.Multiline);
        
        public static Regex SystemResourceRegex = new Regex("~/((sys/)|(views/)|(site\\b))", RegexOptions.IgnoreCase);
        
        private static string _mapsApiIdentifier;
        
        private string _userTheme;
        
        private string _userAccent;
        
        public static Regex CssUrlRegex = new Regex("(?\'Header\'\\burl\\s*\\(\\s*(\\\"|\\\')?)(?\'Name\'\\w+)(?\'Symbol\'\\S)");
        
        private JObject _accentJSON;
        
        public static Regex ThemeStylesheetRegex = new Regex("^touch-theme\\.(?\'Theme\'\\w+)\\.((?\'Accent\'\\w+)\\.)?css$");
        
        private static SortedDictionary<string, string> _themeVariables = null;
        
        public static Regex ThemeVariableRegex = new Regex("(?\'Item\'(?\'Before\'\\w+:\\s*)\\/\\*\\s*(?\'Name\'(@[\\w\\.]+(,\\s*)?)+)\\s*\\*\\/(?\'Value\'.+?))" +
                "(?\'After\'(!important)?;\\s*)$", RegexOptions.Multiline);
        
        public static SortedDictionary<string, ServiceRequestHandler> RequestHandlers = new SortedDictionary<string, ServiceRequestHandler>();
        
        public static Regex ViewPageCompressRegex = new Regex("((\"(DefaultValue)\"\\:(\"[\\s\\S]*?\"))|(\"(Items|Pivots|Fields|Views|ActionGroups|Categ" +
                "ories|Filter|Expressions)\"\\:(\\[\\]))|(\"(Len|CategoryIndex|Rows|Columns|Search|Ite" +
                "msPageSize|Aggregate|OnDemandStyle|TextMode|MaskType|AutoCompletePrefixLength|Da" +
                "taViewPageSize|PageOffset)\"\\:(0))|(\"(CausesValidation|AllowQBE|AllowSorting|Form" +
                "atOnClient|HtmlEncode|RequiresMetaData|RequiresRowCount|ShowInSelector|DataViewS" +
                "how(ActionBar|Description|ViewSelector|PageSize|SearchBar|QuickFind))\"\\:(true))|" +
                "(\"(IsPrimaryKey|ReadOnly|HasDefaultValue|Hidden|AllowLEV|AllowNulls|OnDemand|IsM" +
                "irror|Calculated|CausesCalculate|IsVirtual|AutoSelect|SearchOnStart|ShowInSummar" +
                "y|ItemsLetters|WhenKeySelected|RequiresSiteContentText|RequiresPivot|RequiresAgg" +
                "regates|InTransaction|Wrap|Floating|Collapsed|Label|SupportsCaching|AllowDistinc" +
                "tFieldInFilter|Flat|RequiresMetaData|RequiresRowCount|(DataView(ShowInSummary|Mu" +
                "ltiSelect|ShowModalForms|SearchByFirstLetter|SearchOnStart|ShowRowNumber|AutoHig" +
                "hlightFirstRow|AutoSelectFirstRow)))\"\\:(false))|(\"(AliasName|Tag|FooterText|Tool" +
                "Tip|Watermark|DataFormatString|Copy|HyperlinkFormatString|SourceFields|SearchOpt" +
                "ions|ItemsDataController|ItemsTargetController|ItemsDataView|ItemsDataValueField" +
                "|ItemsDataTextField|ItemsStyle|ItemsNewDataView|OnDemandHandler|Mask|ContextFiel" +
                "ds|Formula|Flow|Label|Configuration|Editor|ItemsDescription|Group|CommandName|Co" +
                "mmandArgument|HeaderText|Description|CssClass|Confirmation|Notify|Key|WhenLastCo" +
                "mmandName|WhenLastCommandArgument|WhenClientScript|WhenTag|WhenHRef|WhenView|Piv" +
                "otDefinitions|Aggregates|PivotDefinitions|Aggregates|ViewType|LastView|StatusBar" +
                "|Icons|LEVs|QuickFindHint|InnerJoinPrimaryKey|SystemFilter|DistinctValueFieldNam" +
                "e|ClientScript|FirstLetters|SortExpression|Template|Tab|Wizard|InnerJoinForeignK" +
                "ey|Expressions|ViewHeaderText|ViewLayout|GroupExpression|(DataView(FilterSource|" +
                "Controller|Id|FilterFields|ShowActionButtons|ShowPager)))\"\\:(\"\"|null))|(\"Type\":\"" +
                "String\")),?");
        
        public static Regex ViewPageCompress2Regex = new Regex(",\\}(,|])");
        
        public virtual JObject DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                {
                    string json = "{}";
                    string filePath = HttpContext.Current.Server.MapPath("~/touch-settings.json");
                    if (File.Exists(filePath))
                    	json = File.ReadAllText(filePath);
                    _defaultSettings = JObject.Parse(json);
                    EnsureJsonProperty(_defaultSettings, "appName", "App On Time");
                    EnsureJsonProperty(_defaultSettings, "map.apiKey", MapsApiIdentifier);
                    EnsureJsonProperty(_defaultSettings, "charts.maxPivotRowCount", MaxPivotRowCount);
                    EnsureJsonProperty(_defaultSettings, "ui.theme.name", "Light");
                    JObject ui = ((JObject)(_defaultSettings["ui"]));
                    EnsureJsonProperty(ui, "theme.accent", "Aquarium");
                    EnsureJsonProperty(ui, "displayDensity.mobile", "Auto");
                    EnsureJsonProperty(ui, "displayDensity.desktop", "Condensed");
                    EnsureJsonProperty(ui, "list.labels.display", "DisplayedBelow");
                    EnsureJsonProperty(ui, "list.initialMode", "SeeAll");
                    EnsureJsonProperty(ui, "menu.location", "toolbar");
                    EnsureJsonProperty(ui, "actions.promote", true);
                    EnsureJsonProperty(ui, "smartDates", true);
                    EnsureJsonProperty(ui, "transitions.style", "");
                    EnsureJsonProperty(ui, "sidebar.when", "Landscape");
                }
                return _defaultSettings;
            }
        }
        
        public static bool EnableMinifiedCss
        {
            get
            {
                return _enableMinifiedCss;
            }
            set
            {
                _enableMinifiedCss = value;
            }
        }
        
        public static bool IsSiteContentEnabled
        {
            get
            {
                return !(String.IsNullOrEmpty(SiteContentControllerName));
            }
        }
        
        public static string SiteContentControllerName
        {
            get
            {
                return Create().GetSiteContentControllerName();
            }
        }
        
        public static string[] SiteContentEditors
        {
            get
            {
                return Create().GetSiteContentEditors();
            }
        }
        
        public static string[] SiteContentDevelopers
        {
            get
            {
                return Create().GetSiteContentDevelopers();
            }
        }
        
        public static bool IsContentEditor
        {
            get
            {
                IPrincipal principal = HttpContext.Current.User;
                foreach (string r in Create().GetSiteContentEditors())
                	if (principal.IsInRole(r))
                    	return true;
                return false;
            }
        }
        
        public static bool IsDeveloper
        {
            get
            {
                IPrincipal principal = HttpContext.Current.User;
                foreach (string r in Create().GetSiteContentDevelopers())
                	if (principal.IsInRole(r))
                    	return true;
                return false;
            }
        }
        
        public static bool IsSafeMode
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                Uri test = request.UrlReferrer;
                if (test == null)
                	test = request.Url;
                return ((test == null) && (test.ToString().Contains("_safemode=true") && DataControllerBase.UserIsInRole(SiteContentDevelopers)));
            }
        }
        
        public virtual int ScheduleCacheDuration
        {
            get
            {
                return 20;
            }
        }
        
        public virtual string Realm
        {
            get
            {
                return Name;
            }
        }
        
        public virtual string Name
        {
            get
            {
                return "App On Time";
            }
        }
        
        public static string MapsApiIdentifier
        {
            get
            {
                if (String.IsNullOrEmpty(_mapsApiIdentifier))
                	_mapsApiIdentifier = WebConfigurationManager.AppSettings["MapsApiIdentifier"];
                return _mapsApiIdentifier;
            }
        }
        
        public virtual int MaxPivotRowCount
        {
            get
            {
                return 250000;
            }
        }
        
        public static ApplicationServices Current
        {
            get
            {
                return Create();
            }
        }
        
        public static bool IsTouchClient
        {
            get
            {
                return false;
            }
        }
        
        public virtual string UserTheme
        {
            get
            {
                if (String.IsNullOrEmpty(_userTheme))
                	LoadTheme();
                return _userTheme;
            }
        }
        
        public virtual string UserAccent
        {
            get
            {
                if (String.IsNullOrEmpty(_userAccent))
                	LoadTheme();
                return _userAccent;
            }
        }
        
        public virtual string GetNavigateUrl()
        {
            return null;
        }
        
        public static void VerifyUrl()
        {
            string navigateUrl = Create().GetNavigateUrl();
            if (!(String.IsNullOrEmpty(navigateUrl)))
            {
                HttpContext current = HttpContext.Current;
                if (!(VirtualPathUtility.ToAbsolute(navigateUrl).Equals(current.Request.RawUrl, StringComparison.CurrentCultureIgnoreCase)))
                	current.Response.Redirect(navigateUrl);
            }
        }
        
        public virtual void RegisterServices()
        {
            CreateStandardMembershipAccounts();
            RouteCollection routes = RouteTable.Routes;
            RegisterIgnoredRoutes(routes);
            RegisterContentServices(RouteTable.Routes);
            // Register service request handlers
            RequestHandlers.Add("getpage", new GetPageServiceRequestHandler());
            RequestHandlers.Add("getpagelist", new GetPageListServiceRequestHandler());
            RequestHandlers.Add("getlistofvalues", new GetListOfValuesServiceRequestHandler());
            RequestHandlers.Add("execute", new ExecuteServiceRequestHandler());
            RequestHandlers.Add("executelist", new ExecuteListServiceRequestHandler());
            RequestHandlers.Add("getcompletionlist", new GetCompletionListServiceRequestHandler());
            RequestHandlers.Add("login", new LoginServiceRequestHandler());
            RequestHandlers.Add("logout", new LogoutServiceRequestHandler());
            RequestHandlers.Add("roles", new RolesServiceRequestHandler());
            RequestHandlers.Add("themes", new ThemesServiceRequestHandler());
            RequestHandlers.Add("savepermalink", new SavePermalinkServiceRequestHandler());
            RequestHandlers.Add("encodepermalink", new EncodePermalinkServiceRequestHandler());
            RequestHandlers.Add("listallpermalinks", new ListAllPermalinksServiceRequestHandler());
            RequestHandlers.Add("getsurvey", new GetSurveyServiceRequestHandler());
            // Find designer port
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IISExpress\\config\\applicationhost.config");
                if (File.Exists(configPath))
                {
                    string content = File.ReadAllText(configPath);
                    Match m = Regex.Match(content, "<site name=\"CodeOnTime\".*?bindingInformation=\"\\*:(?\'Port\'\\d+):localhost\"", RegexOptions.Singleline);
                    if (m.Success)
                    	DesignerPort = m.Groups["Port"].Value;
                }
            }
            finally
            {
                // release resources here
            }
        }
        
        public static void Start()
        {
            Current.InstanceStart();
        }
        
        protected virtual void InstanceStart()
        {
            MyCompany.Services.ApplicationServices.Initialize();
        }
        
        public static void Stop()
        {
            Current.InstanceStop();
        }
        
        protected virtual void InstanceStop()
        {
        }
        
        public static void SessionStart()
        {
            // The line below will prevent intermittent error “Session state has created a session id,
            // but cannot save it because the response was already flushed by the application.”
            string sessionId = HttpContext.Current.Session.SessionID;
            Current.UserSessionStart();
        }
        
        protected virtual void UserSessionStart()
        {
        }
        
        public static void SessionStop()
        {
            Current.UserSessionStop();
        }
        
        protected virtual void UserSessionStop()
        {
        }
        
        public static void Error()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            	Current.HandleError(context, context.Server.GetLastError());
        }
        
        void HandleError(HttpContext context, Exception error)
        {
        }
        
        public virtual void RegisterContentServices(RouteCollection routes)
        {
            GenericRoute.Map(RouteTable.Routes, new PlaceholderHandler(), "placeholder/{FileName}");
        }
        
        public virtual void RegisterIgnoredRoutes(RouteCollection routes)
        {
        }
        
        public static SortedDictionary<string, string> LoadContent()
        {
            SortedDictionary<string, string> content = new SortedDictionary<string, string>();
            Create().LoadContent(HttpContext.Current.Request, HttpContext.Current.Response, content);
            string rawContent = null;
            if (content.TryGetValue("File", out rawContent))
            {
                // find the head
                Match headMatch = Regex.Match(rawContent, "<head>([\\s\\S]+?)</head>");
                if (headMatch.Success)
                {
                    string head = headMatch.Groups[1].Value;
                    head = Regex.Replace(head, "\\s*<meta charset=\".+\"\\s*/?>\\s*", String.Empty);
                    content["Head"] = Regex.Replace(head, "\\s*<title>([\\S\\s]*?)</title>\\s*", String.Empty);
                    // find the title
                    Match titleMatch = Regex.Match(head, "<title>(?\'Title\'[\\S\\s]+?)</title>");
                    if (titleMatch.Success)
                    {
                        string title = titleMatch.Groups["Title"].Value;
                        content["PageTitle"] = title;
                        content["PageTitleContent"] = title;
                    }
                    // find "about"
                    Match aboutMatch = Regex.Match(head, "<meta\\s+name\\s*=\\s*\"description\"\\s+content\\s*=\\s*\"([\\s\\S]+?)\"\\s*/>");
                    if (aboutMatch.Success)
                    	content["About"] = HttpUtility.HtmlDecode(aboutMatch.Groups[1].Value);
                }
                // find the body
                Match bodyMatch = Regex.Match(rawContent, "<body(?\'Attr\'[\\s\\S]*?)>(?\'Body\'[\\s\\S]+?)</body>");
                if (bodyMatch.Success)
                {
                    content["PageContent"] = EnrichData(bodyMatch.Groups["Body"].Value);
                    content["BodyAttributes"] = bodyMatch.Groups["Attr"].Value;
                }
                else
                	content["PageContent"] = EnrichData(rawContent);
            }
            return content;
        }
        
        static string EnrichData(string body)
        {
            return Regex.Replace(body, "(<script[^>]*data-type=\"\\$app\\.execute\"[^>]*>(?<Script>(.|\\n)*?)<\\/script>)", DoEnrichData);
        }
        
        static string DoEnrichData(Match m)
        {
            try
            {
                string json = m.Groups["Script"].Value.Trim().Trim(')', '(', ';');
                JObject obj = JObject.Parse(json);
                PageRequest request = new PageRequest();
                request.Controller = ((string)(obj["controller"]));
                request.View = ((string)(obj["view"]));
                request.PageIndex = Convert.ToInt32(obj["pageIndex"]);
                request.PageSize = Convert.ToInt32(obj["pageSize"]);
                if (request.PageSize == 0)
                	request.PageSize = 100;
                request.SortExpression = ((string)(obj["sortExpression"]));
                JArray metadataFilter = ((JArray)(obj["metadataFilter"]));
                if (metadataFilter != null)
                	request.MetadataFilter = metadataFilter.ToObject<string[]>();
                else
                	request.MetadataFilter = new string[] {
                            "fields"};
                request.RequiresMetaData = true;
                ViewPage page = ControllerFactory.CreateDataController().GetPage(request.Controller, request.View, request);
                string output = ApplicationServices.CompressViewPageJsonOutput(JsonConvert.SerializeObject(page));
                object doFormat = obj["format"];
                if (doFormat == null)
                	doFormat = "true";
                object id = obj["id"];
                if (id == null)
                	id = request.Controller;
                return String.Format("<script>$app.data({{\"id\":\"{0}\",\"format\":{1},\"d\":{2}}});</script>", id, Convert.ToBoolean(doFormat).ToString().ToLower(), output);
            }
            catch (Exception ex)
            {
                return (("<div class=\"well text-danger\">" + ex.Message) 
                            + "</div>");
            }
        }
        
        public virtual string GetSiteContentControllerName()
        {
            return null;
        }
        
        public virtual string GetSiteContentViewId()
        {
            return "editForm1";
        }
        
        public virtual string[] GetSiteContentEditors()
        {
            return new string[] {
                    "Administrators",
                    "Content Editors",
                    "Developers"};
        }
        
        public virtual string[] GetSiteContentDevelopers()
        {
            return new string[] {
                    "Administrators",
                    "Developers"};
        }
        
        public virtual void AfterAction(ActionArgs args, ActionResult result)
        {
        }
        
        public virtual void BeforeAction(ActionArgs args, ActionResult result)
        {
            if (args.Controller == SiteContentControllerName)
            {
                bool userIsDeveloper = IsDeveloper;
                if ((!(IsContentEditor) || !(userIsDeveloper)) || (args.Values == null))
                	throw new HttpException(403, "Forbidden");
                FieldValue id = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.SiteContentId));
                FieldValue path = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.Path));
                FieldValue fileName = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.DataFileName));
                FieldValue text = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.Text));
                // verify "Path" access
                if ((path == null) || (fileName == null))
                	throw new HttpException(403, "Forbidden");
                if (((path.Value != null) && path.Value.ToString().StartsWith("sys/", StringComparison.CurrentCultureIgnoreCase)) && !(userIsDeveloper))
                	throw new HttpException(403, "Forbidden");
                if (((path.OldValue != null) && path.OldValue.ToString().StartsWith("sys/", StringComparison.CurrentCultureIgnoreCase)) && !(userIsDeveloper))
                	throw new HttpException(403, "Forbidden");
                // convert and parse "Text" as needed
                if ((text != null) && args.CommandName != "Delete")
                {
                    string s = Convert.ToString(text.Value);
                    if (s == "$Text")
                    {
                        string fullPath = Convert.ToString(path.Value);
                        if (!(String.IsNullOrEmpty(fullPath)))
                        	fullPath = (fullPath + "/");
                        fullPath = (fullPath + Convert.ToString(fileName.Value));
                        if (!(fullPath.StartsWith("/")))
                        	fullPath = ("/" + fullPath);
                        if (!(fullPath.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase)))
                        	fullPath = (fullPath + ".html");
                        string physicalPath = HttpContext.Current.Server.MapPath(("~" + fullPath));
                        if (!(File.Exists(physicalPath)))
                        {
                            physicalPath = HttpContext.Current.Server.MapPath(("~" + fullPath.Replace("-", String.Empty)));
                            if (!(File.Exists(physicalPath)))
                            	physicalPath = null;
                        }
                        if (!(String.IsNullOrEmpty(physicalPath)))
                        	text.NewValue = File.ReadAllText(physicalPath);
                    }
                }
            }
        }
        
        public virtual string SiteContentFieldName(SiteContentFields field)
        {
            return field.ToString();
        }
        
        public virtual string ReadSiteContentString(string relativePath)
        {
            byte[] data = ReadSiteContentBytes(relativePath);
            if (data == null)
            	return null;
            return Encoding.UTF8.GetString(data);
        }
        
        public virtual byte[] ReadSiteContentBytes(string relativePath)
        {
            SiteContentFile f = ReadSiteContent(relativePath);
            if (f == null)
            	return null;
            return f.Data;
        }
        
        public virtual SiteContentFile ReadSiteContent(string relativePath)
        {
            HttpContext context = HttpContext.Current;
            SiteContentFile f = ((SiteContentFile)(context.Items[relativePath]));
            if (f == null)
            	f = ((SiteContentFile)(context.Cache[relativePath]));
            if (f == null)
            {
                string path = relativePath;
                string fileName = relativePath;
                int index = relativePath.LastIndexOf("/");
                if (index >= 0)
                {
                    fileName = path.Substring((index + 1));
                    path = relativePath.Substring(0, index);
                }
                else
                	path = null;
                SiteContentFileList files = ReadSiteContent(path, fileName, 1);
                if (files.Count == 1)
                {
                    f = files[0];
                    context.Items[relativePath] = f;
                    if (f.CacheDuration > 0)
                    	context.Cache.Add(relativePath, f, null, DateTime.Now.AddSeconds(f.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
            }
            return f;
        }
        
        public virtual SiteContentFileList ReadSiteContent(string relativePath, string fileName)
        {
            return ReadSiteContent(relativePath, fileName, Int32.MaxValue);
        }
        
        public virtual SiteContentFileList ReadSiteContent(string relativePath, string fileName, int maxCount)
        {
            SiteContentFileList result = new SiteContentFileList();
            if (IsSafeMode)
            	return result;
            // prepare a filter
            string dataFileNameField = SiteContentFieldName(SiteContentFields.DataFileName);
            string pathField = SiteContentFieldName(SiteContentFields.Path);
            List<string> filter = new List<string>();
            string pathFilter = null;
            if (!(String.IsNullOrEmpty(relativePath)))
            {
                pathFilter = "{0}:={1}";
                int firstWildcardIndex = relativePath.IndexOf("%");
                if (firstWildcardIndex >= 0)
                {
                    int lastWildcardIndex = relativePath.LastIndexOf("%");
                    pathFilter = "{0}:$contains${1}";
                    if (firstWildcardIndex == lastWildcardIndex)
                    	if (firstWildcardIndex == 0)
                        {
                            pathFilter = "{0}:$endswith${1}";
                            relativePath = relativePath.Substring(1);
                        }
                        else
                        	if (lastWildcardIndex == (relativePath.Length - 1))
                            {
                                pathFilter = "{0}:$beginswith${1}";
                                relativePath = relativePath.Substring(0, lastWildcardIndex);
                            }
                }
            }
            else
            	pathFilter = "{0}:=null";
            string fileNameFilter = null;
            if (!(String.IsNullOrEmpty(fileName)) && !((fileName == "%")))
            {
                fileNameFilter = "{0}:={1}";
                int firstWildcardIndex = fileName.IndexOf("%");
                if (firstWildcardIndex >= 0)
                {
                    int lastWildcardIndex = fileName.LastIndexOf("%");
                    fileNameFilter = "{0}:$contains${1}";
                    if (firstWildcardIndex == lastWildcardIndex)
                    	if (firstWildcardIndex == 0)
                        {
                            fileNameFilter = "{0}:$endswith${1}";
                            fileName = fileName.Substring(1);
                        }
                        else
                        	if (lastWildcardIndex == (fileName.Length - 1))
                            {
                                fileNameFilter = "{0}:$beginswith${1}";
                                fileName = fileName.Substring(0, lastWildcardIndex);
                            }
                }
            }
            if (!(String.IsNullOrEmpty(pathFilter)) || !(String.IsNullOrEmpty(fileNameFilter)))
            {
                filter.Add("_match_:$all$");
                if (!(String.IsNullOrEmpty(pathFilter)))
                	filter.Add(String.Format(pathFilter, pathField, DataControllerBase.ValueToString(relativePath)));
                if (fileName != null && !((fileName == "%")))
                {
                    filter.Add(String.Format(fileNameFilter, dataFileNameField, DataControllerBase.ValueToString(fileName)));
                    if (String.IsNullOrEmpty(Path.GetExtension(fileName)) && (String.IsNullOrEmpty(relativePath) || (!(relativePath.StartsWith("sys/", StringComparison.OrdinalIgnoreCase)) || relativePath.StartsWith("sys/controls", StringComparison.OrdinalIgnoreCase))))
                    {
                        filter.Add("_match_:$all$");
                        if (!(String.IsNullOrEmpty(pathFilter)))
                        	filter.Add(String.Format(pathFilter, pathField, DataControllerBase.ValueToString(relativePath)));
                        filter.Add(String.Format(fileNameFilter, dataFileNameField, DataControllerBase.ValueToString((Path.GetFileNameWithoutExtension(fileName).Replace("-", String.Empty) + ".html"))));
                    }
                }
            }
            //  determine user identity
            HttpContext context = HttpContext.Current;
            string userName = String.Empty;
            bool isAuthenticated = false;
            IPrincipal user = context.User;
            if (user != null)
            {
                userName = user.Identity.Name.ToLower();
                isAuthenticated = user.Identity.IsAuthenticated;
            }
            // enumerate site content files
            PageRequest r = new PageRequest();
            r.Controller = GetSiteContentControllerName();
            r.View = GetSiteContentViewId();
            r.RequiresSiteContentText = true;
            r.PageSize = Int32.MaxValue;
            r.Filter = filter.ToArray();
            IDataEngine engine = ControllerFactory.CreateDataEngine();
            DataControllerBase controller = ((DataControllerBase)(engine));
            controller.AllowPublicAccess = true;
            IDataReader reader = engine.ExecuteReader(r);
            SortedDictionary<string, SiteContentFile> blobsToResolve = new SortedDictionary<string, SiteContentFile>();
            // verify optional SiteContent fields
            SortedDictionary<string, string> fieldDictionary = new SortedDictionary<string, string>();
            for (int i = 0; (i < reader.FieldCount); i++)
            {
                string fieldName = reader.GetName(i);
                fieldDictionary[fieldName] = fieldName;
            }
            string rolesField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Roles), out rolesField);
            string usersField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Users), out usersField);
            string roleExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.RoleExceptions), out roleExceptionsField);
            string userExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.UserExceptions), out userExceptionsField);
            string cacheProfileField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.CacheProfile), out cacheProfileField);
            string scheduleField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Schedule), out scheduleField);
            string scheduleExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.ScheduleExceptions), out scheduleExceptionsField);
            DataField dataField = controller.CreateViewPage().FindField(SiteContentFieldName(SiteContentFields.Data));
            string blobHandler = dataField.OnDemandHandler;
            WorkflowRegister wr = WorkflowRegister.GetCurrent(relativePath);
            // read SiteContent files
            while (reader.Read())
            {
                // verify user access rights
                bool include = true;
                if (!(String.IsNullOrEmpty(rolesField)))
                {
                    string roles = Convert.ToString(reader[rolesField]);
                    if (!(String.IsNullOrEmpty(roles)) && !((roles == "?")))
                    	if ((roles == "*") && !(isAuthenticated))
                        	include = false;
                        else
                        	if (!(isAuthenticated) || (!((roles == "*")) && !(DataControllerBase.UserIsInRole(roles))))
                            	include = false;
                }
                if (include && !(String.IsNullOrEmpty(usersField)))
                {
                    string users = Convert.ToString(reader[usersField]);
                    if (!(String.IsNullOrEmpty(users)) && (Array.IndexOf(users.ToLower().Split(new char[] {
                                                ','}, StringSplitOptions.RemoveEmptyEntries), userName) == -1))
                    	include = false;
                }
                if (include && !(String.IsNullOrEmpty(roleExceptionsField)))
                {
                    string roleExceptions = Convert.ToString(reader[roleExceptionsField]);
                    if (!(String.IsNullOrEmpty(roleExceptions)) && (isAuthenticated && ((roleExceptions == "*") || DataControllerBase.UserIsInRole(roleExceptions))))
                    	include = false;
                }
                if (include && !(String.IsNullOrEmpty(userExceptionsField)))
                {
                    string userExceptions = Convert.ToString(reader[userExceptionsField]);
                    if (!(String.IsNullOrEmpty(userExceptions)) && !((Array.IndexOf(userExceptions.ToLower().Split(new char[] {
                                                ','}, StringSplitOptions.RemoveEmptyEntries), userName) == -1)))
                    	include = false;
                }
                string physicalName = Convert.ToString(reader[dataFileNameField]);
                string physicalPath = Convert.ToString(reader[SiteContentFieldName(SiteContentFields.Path)]);
                // check if the content object is a part of a workflow
                if (((wr != null) && wr.Enabled) && !(wr.IsMatch(physicalPath, physicalName)))
                	include = false;
                string schedule = null;
                string scheduleExceptions = null;
                // check if the content object is on schedule
                if (include && (String.IsNullOrEmpty(physicalPath) || !(physicalPath.StartsWith("sys/schedules/"))))
                {
                    if (!(String.IsNullOrEmpty(scheduleField)))
                    	schedule = Convert.ToString(reader[scheduleField]);
                    if (!(String.IsNullOrEmpty(scheduleExceptionsField)))
                    	scheduleExceptions = Convert.ToString(reader[scheduleExceptionsField]);
                    if (!(String.IsNullOrEmpty(schedule)) || !(String.IsNullOrEmpty(scheduleExceptions)))
                    {
                        string scheduleStatusKey = String.Format("ScheduleStatus|{0}|{1}", schedule, scheduleExceptions);
                        ScheduleStatus status = ((ScheduleStatus)(context.Items[scheduleStatusKey]));
                        if (status == null)
                        	status = ((ScheduleStatus)(context.Cache[scheduleStatusKey]));
                        bool scheduleStatusChanged = false;
                        if (status == null)
                        {
                            if (!(String.IsNullOrEmpty(schedule)) && !(schedule.Contains("+")))
                            	schedule = ReadSiteContentString(("sys/schedules%/" + schedule));
                            if (!(String.IsNullOrEmpty(scheduleExceptions)) && !(scheduleExceptions.Contains("+")))
                            	scheduleExceptions = ReadSiteContentString(("sys/schedules%/" + scheduleExceptions));
                            if (!(String.IsNullOrEmpty(schedule)) || !(String.IsNullOrEmpty(scheduleExceptions)))
                            	status = Scheduler.Test(schedule, scheduleExceptions);
                            else
                            {
                                status = new ScheduleStatus();
                                status.Success = true;
                                status.NextTestDate = DateTime.MaxValue;
                            }
                            context.Items[scheduleStatusKey] = status;
                            scheduleStatusChanged = true;
                        }
                        else
                        	if (DateTime.Now > status.NextTestDate)
                            {
                                status = Scheduler.Test(status.Schedule, status.Exceptions);
                                context.Items[scheduleStatusKey] = status;
                                scheduleStatusChanged = true;
                            }
                        if (scheduleStatusChanged)
                        	context.Cache.Add(scheduleStatusKey, status, null, DateTime.Now.AddSeconds(ScheduleCacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        if (!(status.Success))
                        	include = false;
                    }
                }
                // create a file instance
                if (include)
                {
                    string siteContentIdField = SiteContentFieldName(SiteContentFields.SiteContentId);
                    SiteContentFile f = new SiteContentFile();
                    f.Id = reader[siteContentIdField];
                    f.Name = fileName;
                    f.PhysicalName = physicalName;
                    if (String.IsNullOrEmpty(f.Name) || f.Name.Contains("%"))
                    	f.Name = f.PhysicalName;
                    f.Path = physicalPath;
                    f.ContentType = Convert.ToString(reader[SiteContentFieldName(SiteContentFields.DataContentType)]);
                    f.Schedule = schedule;
                    f.ScheduleExceptions = scheduleExceptions;
                    if (!(String.IsNullOrEmpty(cacheProfileField)))
                    {
                        string cacheProfile = Convert.ToString(reader[cacheProfileField]);
                        if (!(String.IsNullOrEmpty(cacheProfile)))
                        {
                            f.CacheProfile = cacheProfile;
                            cacheProfile = ReadSiteContentString(("sys/cache-profiles/" + cacheProfile));
                            if (!(String.IsNullOrEmpty(cacheProfile)))
                            {
                                Match m = NameValueListRegex.Match(cacheProfile);
                                while (m.Success)
                                {
                                    string n = m.Groups["Name"].Value.ToLower();
                                    string v = m.Groups["Value"].Value;
                                    if (n == "duration")
                                    {
                                        int duration = 0;
                                        if (Int32.TryParse(v, out duration))
                                        {
                                            f.CacheDuration = duration;
                                            f.CacheLocation = HttpCacheability.ServerAndPrivate;
                                        }
                                    }
                                    else
                                    	if (n == "location")
                                        	try
                                            {
                                                f.CacheLocation = ((HttpCacheability)(TypeDescriptor.GetConverter(typeof(HttpCacheability)).ConvertFromString(v)));
                                            }
                                            catch (Exception )
                                            {
                                            }
                                        else
                                        	if (n == "varybyheaders")
                                            	f.CacheVaryByHeaders = v.Split(new char[] {
                                                            ',',
                                                            ';'}, StringSplitOptions.RemoveEmptyEntries);
                                            else
                                            	if (n == "varybyparams")
                                                	f.CacheVaryByParams = v.Split(new char[] {
                                                                ',',
                                                                ';'}, StringSplitOptions.RemoveEmptyEntries);
                                                else
                                                	if (n == "nostore")
                                                    	f.CacheNoStore = (v.ToLower() == "true");
                                    m = m.NextMatch();
                                }
                            }
                        }
                    }
                    object textString = reader[SiteContentFieldName(SiteContentFields.Text)];
                    if (DBNull.Value.Equals(textString) || !(f.IsText))
                    {
                        string blobKey = String.Format("{0}=o|{1}", blobHandler, f.Id);
                        if (f.CacheDuration > 0)
                        	f.Data = ((byte[])(HttpContext.Current.Cache[blobKey]));
                        if (f.Data == null)
                        	blobsToResolve[blobKey] = f;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(f.ContentType))
                        	if (Regex.IsMatch(((string)(textString)), "</\\w+\\s*>"))
                            	f.ContentType = "text/xml";
                            else
                            	f.ContentType = "text/plain";
                        f.Data = Encoding.UTF8.GetBytes(((string)(textString)));
                    }
                    result.Add(f);
                    if (result.Count == maxCount)
                    	break;
                }
            }
            reader.Close();
            foreach (string blobKey in blobsToResolve.Keys)
            {
                SiteContentFile f = blobsToResolve[blobKey];
                // download blob content
                try
                {
                    f.Data = Blob.Read(blobKey);
                    if (f.CacheDuration > 0)
                    	HttpContext.Current.Cache.Add(blobKey, f.Data, null, DateTime.Now.AddSeconds(f.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
                catch (Exception ex)
                {
                    f.Error = ex.Message;
                }
            }
            return result;
        }
        
        public virtual bool IsSystemResource(HttpRequest request)
        {
            return SystemResourceRegex.IsMatch(request.AppRelativeCurrentExecutionFilePath);
        }
        
        public virtual void LoadContent(HttpRequest request, HttpResponse response, SortedDictionary<string, string> content)
        {
            if (IsSystemResource(request))
            	return;
            string text = null;
            bool tryFileSystem = true;
            if (IsSiteContentEnabled)
            {
                string fileName = HttpUtility.UrlDecode(request.Url.Segments[(request.Url.Segments.Length - 1)]);
                string path = request.CurrentExecutionFilePath.Substring(request.ApplicationPath.Length);
                if ((fileName == "/") && String.IsNullOrEmpty(path))
                	fileName = "index";
                else
                {
                    path = path.Substring(0, (path.Length - fileName.Length));
                    if (path.EndsWith("/"))
                    	path = path.Substring(0, (path.Length - 1));
                }
                if (String.IsNullOrEmpty(path))
                	path = null;
                SiteContentFileList files = ReadSiteContent(path, fileName, 1);
                if (files.Count > 0)
                {
                    SiteContentFile f = files[0];
                    if (f.ContentType == "text/html")
                    {
                        text = f.Text;
                        tryFileSystem = false;
                    }
                    else
                    {
                        if (f.CacheDuration > 0)
                        {
                            DateTime expires = DateTime.Now.AddSeconds(f.CacheDuration);
                            response.Cache.SetExpires(expires);
                            response.Cache.SetCacheability(f.CacheLocation);
                            if (f.CacheVaryByParams != null)
                            	foreach (string header in f.CacheVaryByParams)
                                	response.Cache.VaryByParams[header] = true;
                            if (f.CacheVaryByHeaders != null)
                            	foreach (string header in f.CacheVaryByHeaders)
                                	response.Cache.VaryByHeaders[header] = true;
                            if (f.CacheNoStore)
                            	response.Cache.SetNoStore();
                        }
                        response.ContentType = f.ContentType;
                        response.AddHeader("Content-Disposition", ("filename=" + HttpUtility.UrlEncode(f.PhysicalName)));
                        response.OutputStream.Write(f.Data, 0, f.Data.Length);
                        try
                        {
                            response.Flush();
                        }
                        catch (Exception )
                        {
                        }
                        response.End();
                    }
                }
            }
            if (tryFileSystem)
            {
                string filePath = request.PhysicalPath;
                string fileExtension = Path.GetExtension(filePath);
                if (!((fileExtension.ToLower() == ".html")) && File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    response.AddHeader("Content-Disposition", ("filename=" + HttpUtility.UrlEncode(fileName)));
                    System.DateTime expires = DateTime.Now.AddSeconds(((60 * 60) 
                                    * 24));
                    response.Cache.SetExpires(expires);
                    response.Cache.SetCacheability(HttpCacheability.Public);
                    byte[] data = File.ReadAllBytes(filePath);
                    response.OutputStream.Write(data, 0, data.Length);
                    try
                    {
                        response.Flush();
                    }
                    catch (Exception )
                    {
                    }
                    response.End();
                }
                if (!(String.IsNullOrEmpty(fileExtension)))
                	filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                filePath = (filePath + ".html");
                if (File.Exists(filePath))
                	text = File.ReadAllText(filePath);
                else
                	if (Path.GetFileNameWithoutExtension(filePath).Contains("-"))
                    {
                        filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath).Replace("-", String.Empty));
                        if (File.Exists(filePath))
                        	text = File.ReadAllText(filePath);
                    }
                if (text != null)
                	text = Localizer.Replace("Pages", filePath, text);
            }
            if (text != null)
            {
                text = Regex.Replace(text, "<!--[\\s\\S]+?-->\\s*", String.Empty);
                content["File"] = text;
            }
        }
        
        public virtual void CreateStandardMembershipAccounts()
        {
            // Create a separate code file with a definition of the partial class ApplicationServices overriding
            // this method to prevent automatic registration of 'admin' and 'user'. Do not change this file directly.
            RegisterStandardMembershipAccounts();
        }
        
        public virtual bool RequiresAuthentication(HttpRequest request)
        {
            return request.Path.EndsWith("Export.ashx", StringComparison.CurrentCultureIgnoreCase);
        }
        
        public virtual bool AuthenticateRequest(HttpContext context)
        {
            return false;
        }
        
        public virtual void RedirectToLoginPage()
        {
            HttpCookie saas = HttpContext.Current.Response.Cookies["COT_SaaS"];
            if (saas != null)
            {
                OAuthHandler handler = OAuthHandlerFactory.Create(saas.Value);
                if (handler != null)
                {
                    handler.StartPage = HttpContext.Current.Request.Url.AbsolutePath;
                    handler.RedirectToLoginPage();
                    return;
                }
            }
            FormsAuthentication.RedirectToLoginPage();
        }
        
        public virtual object AuthenticateUser(string username, string password, bool createPersistentCookie)
        {
            HttpResponse response = HttpContext.Current.Response;
            if (password.StartsWith("token:"))
            {
                // validate token login
                try
                {
                    string key = password.Substring(6);
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(key);
                    if (ValidateTicket(ticket))
                    {
                        MembershipUser user = Membership.GetUser(ticket.Name);
                        if (user != null)
                        {
                            InvalidateTicket(ticket);
                            FormsAuthentication.SetAuthCookie(user.UserName, createPersistentCookie);
                            return CreateTicket(user);
                        }
                    }
                }
                catch (Exception )
                {
                }
            }
            else
            {
                // login user
                if (UserLogin(username, password, createPersistentCookie))
                {
                    MembershipUser user = Membership.GetUser(username);
                    if (user != null)
                    	return CreateTicket(user);
                }
            }
            return false;
        }
        
        public virtual UserTicket CreateTicket(MembershipUser user)
        {
            int timeout = (60 
                        * (24 * 7));
            JToken jTimeout = DefaultSettings["TokenExpiration"];
            if (jTimeout != null)
            	timeout = (((int)(jTimeout)) * 60);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(user.UserName, false, timeout);
            return new UserTicket(user, FormsAuthentication.Encrypt(ticket));
        }
        
        public virtual bool ValidateTicket(FormsAuthenticationTicket ticket)
        {
            return !(((ticket == null) || (ticket.Expired || String.IsNullOrEmpty(ticket.Name))));
        }
        
        public virtual void InvalidateTicket(FormsAuthenticationTicket ticket)
        {
        }
        
        public virtual bool UserLogin(string username, string password, bool createPersistentCookie)
        {
            if (Membership.ValidateUser(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
                return true;
            }
            else
            	return false;
        }
        
        public virtual void UserLogout()
        {
            FormsAuthentication.SignOut();
        }
        
        public virtual string[] UserRoles()
        {
            return Roles.GetRolesForUser();
        }
        
        public virtual JObject UserThemes()
        {
            JObject lists = new JObject();
            JArray themes = new JArray();
            JArray accents = new JArray();
            lists["themes"] = themes;
            lists["accents"] = accents;
            string touchPath = HttpContext.Current.Server.MapPath("~/touch");
            foreach (string f in Directory.GetFiles(touchPath, "touch-theme.*.json"))
            	themes.Add(JObject.Parse(File.ReadAllText(f)));
            foreach (string f in Directory.GetFiles(touchPath, "touch-accent.*.json"))
            	accents.Add(JObject.Parse(File.ReadAllText(f)));
            return lists;
        }
        
        public virtual JObject UserSettings(Page p)
        {
            JObject settings = new JObject(DefaultSettings);
            if (settings["membership"] == null)
            	settings["membership"] = new JObject();
            string userKey = String.Empty;
            MembershipUser user = Membership.GetUser();
            if (user != null)
            {
                userKey = Convert.ToString(user.ProviderUserKey);
                if (!(String.IsNullOrEmpty(user.Comment)))
                {
                    Match m = Regex.Match(user.Comment, "\\bSource:\\s*\\b(?\'Value\'\\w+)\\b", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        OAuthHandler handler = OAuthHandlerFactory.Create(m.Groups["Value"].Value);
                        if (handler != null)
                        	settings["membership"]["profile"] = handler.GetUserProfile();
                    }
                }
            }
            settings["appInfo"] = string.Join("|", new string[] {
                        Name,
                        HttpContext.Current.User.Identity.Name,
                        userKey});
            settings["ui"]["theme"]["name"] = UserTheme;
            settings["ui"]["theme"]["accent"] = UserAccent;
            settings["defaultUI"] = "Desktop";
            return settings;
        }
        
        public virtual string UserHomePageUrl()
        {
            return "~/Pages/Home.aspx";
        }
        
        public virtual string UserPictureString(MembershipUser user)
        {
            return String.Empty;
        }
        
        public virtual Image UserPictureImage(MembershipUser user)
        {
            return null;
        }
        
        public virtual string UserPictureFilePath(MembershipUser user)
        {
            return null;
        }
        
        public virtual string UserPictureUrl(MembershipUser user)
        {
            return null;
        }
        
        public static ApplicationServices Create()
        {
            return new ApplicationServices();
        }
        
        public static bool UserIsAuthorizedToAccessResource(string path, string roles)
        {
            return !(Create().ResourceAuthorizationIsRequired(path, roles));
        }
        
        public virtual bool ResourceAuthorizationIsRequired(string path, string roles)
        {
            if (roles == null)
            	roles = String.Empty;
            else
            	roles = roles.Trim();
            bool requiresAuthorization = false;
            bool isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (String.IsNullOrEmpty(roles) && !(isAuthenticated))
            	requiresAuthorization = true;
            if (!(String.IsNullOrEmpty(roles)) && !((roles == "?")))
            	if (roles == "*")
                {
                    if (!(isAuthenticated))
                    	requiresAuthorization = true;
                }
                else
                	if (!(isAuthenticated) || !(DataControllerBase.UserIsInRole(roles)))
                    	requiresAuthorization = true;
            if (path == FormsAuthentication.LoginUrl)
            {
                requiresAuthorization = false;
                if (!(isAuthenticated) && !((HttpContext.Current.Request.QueryString["_autoLogin"] == "false")))
                {
                    OAuthHandler handler = OAuthHandlerFactory.CreateAutoLogin();
                    if (handler != null)
                    {
                        HttpContext.Current.Response.Cookies.Set(new HttpCookie("COT_SaaS", handler.GetHandlerName()));
                        requiresAuthorization = true;
                    }
                }
            }
            return requiresAuthorization;
        }
        
        public static void RegisterStandardMembershipAccounts()
        {
            // Confirm existence of schema version
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["LocalSqlServer"];
            if (css == null)
            	css = ConfigurationManager.ConnectionStrings["MyCompany"];
            if ((css != null) && (css.ProviderName == "System.Data.SqlClient"))
            {
                int count = 0;
                using (SqlText sql = new SqlText("select count(*) from dbo.aspnet_SchemaVersions where Feature in (\'common\', \'membe" +
                        "rship\', \'role manager\')", css.Name))
                	count = ((int)(sql.ExecuteScalar()));
                if (count == 0)
                {
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'common\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'membership\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'role manager\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                }
            }
            MembershipUser admin = Membership.GetUser("admin");
            if ((admin != null) && admin.IsLockedOut)
            	admin.UnlockUser();
            MembershipUser user = Membership.GetUser("user");
            if ((user != null) && user.IsLockedOut)
            	user.UnlockUser();
            if (Membership.GetUser("admin") == null)
            {
                MembershipCreateStatus status;
                admin = Membership.CreateUser("admin", "admin123%", "admin@MyCompany.com", "ASP.NET", "Code OnTime", true, out status);
                user = Membership.CreateUser("user", "user123%", "user@MyCompany.com", "ASP.NET", "Code OnTime", true, out status);
                Roles.CreateRole("Administrators");
                Roles.CreateRole("Users");
                Roles.AddUserToRole(admin.UserName, "Users");
                Roles.AddUserToRole(user.UserName, "Users");
                Roles.AddUserToRole(admin.UserName, "Administrators");
            }
        }
        
        public static void RegisterCssLinks(Page p)
        {
            foreach (Control c in p.Header.Controls)
            	if (c is HtmlLink)
                {
                    HtmlLink l = ((HtmlLink)(c));
                    if (l.ID == "MyCompanyTheme")
                    	return;
                    if (l.Href.Contains("_Theme_Aquarium.css"))
                    {
                        l.ID = "MyCompanyTheme";
                        if (ApplicationServices.IsTouchClient)
                        {
                            ApplicationServices services = ApplicationServices.Current;
                            string jqmCss = String.Format("jquery.mobile-{0}.min.css", ApplicationServices.JqmVersion);
                            l.Href = ("~/touch/" + jqmCss);
                            HtmlMeta meta = new HtmlMeta();
                            meta.Attributes["name"] = "viewport";
                            meta.Attributes["content"] = "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no";
                            p.Header.Controls.AddAt(0, meta);
                            if (ApplicationServices.EnableMinifiedCss)
                            {
                                l.Href = p.ResolveUrl(string.Format("~/appservices/stylesheet-{0}.min.css?_t={1}.{2}&_cf=", ApplicationServices.Version, services.UserTheme, services.UserAccent));
                                l.Attributes["class"] = "app-theme";
                            }
                            else
                            	foreach (string stylesheet in services.EnumerateTouchUIStylesheets())
                                {
                                    string cssName = Path.GetFileName(stylesheet);
                                    if (!(cssName.StartsWith("jquery.mobile")) && !(cssName.StartsWith("bootstrap")))
                                    {
                                        HtmlLink cssLink = new HtmlLink();
                                        cssLink.Href = String.Format("{0}?{1}", stylesheet.Replace('\\', '/'), ApplicationServices.Version);
                                        if (cssName.StartsWith("touch-theme."))
                                        	cssLink.Attributes["class"] = "app-theme";
                                        cssLink.Attributes["type"] = "text/css";
                                        cssLink.Attributes["rel"] = "stylesheet";
                                        p.Header.Controls.Add(cssLink);
                                    }
                                }
                            List<Control> removeList = new List<Control>();
                            foreach (Control c2 in p.Header.Controls)
                            	if (c2 is HtmlLink)
                                {
                                    l = ((HtmlLink)(c2));
                                    if (l.Href.Contains("App_Themes/"))
                                    	removeList.Add(l);
                                }
                            foreach (Control c2 in removeList)
                            	p.Header.Controls.Remove(c2);
                            return;
                        }
                        else
                        	if (!(l.Href.Contains("?")))
                            	l.Href = (l.Href + String.Format("?{0}", ApplicationServices.Version));
                        return;
                    }
                }
        }
        
        private void LoadTheme()
        {
            string theme = String.Empty;
            if (HttpContext.Current != null)
            {
                HttpCookie themeCookie = HttpContext.Current.Request.Cookies[(".COTTHEME" + BusinessRules.UserName)];
                if (themeCookie != null)
                	theme = themeCookie.Value;
            }
            if (!(String.IsNullOrEmpty(theme)) && theme.Contains('.'))
            {
                theme = theme.Replace(" ", String.Empty);
                string[] parts = theme.Split('.');
                _userTheme = parts[0];
                _userAccent = parts[1];
            }
            else
            {
                _userTheme = ((string)(DefaultSettings["ui"]["theme"]["name"]));
                _userAccent = ((string)(DefaultSettings["ui"]["theme"]["accent"]));
            }
        }
        
        protected virtual bool AllowTouchUIStylesheet(string name)
        {
            return !(Regex.IsMatch(name, "^(touch|bootstrap|jquery\\.mobile)"));
        }
        
        public virtual List<string> EnumerateTouchUIStylesheets()
        {
            List<string> stylesheets = new List<string>();
            stylesheets.Add(string.Format("~\\touch\\jquery.mobile-{0}.min.css", ApplicationServices.JqmVersion));
            if (EnableMinifiedCss)
            {
                stylesheets.Add("~\\touch\\touch.min.css");
                stylesheets.Add("~\\touch\\touch-charts.min.css");
                stylesheets.Add("~\\touch\\bootstrap.min.css");
            }
            else
            {
                stylesheets.Add("~\\touch\\touch.css");
                stylesheets.Add("~\\touch\\touch-charts.css");
                stylesheets.Add("~\\touch\\bootstrap.css");
            }
            stylesheets.Add(String.Format("~\\appservices\\touch-theme.{0}.{1}.css", UserTheme, UserAccent));
            // enumerate custom css files
            string mobilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "touch");
            IEnumerable<string> paths = Directory.GetFiles(mobilePath, "*.css");
            string cssPath = Path.Combine(HttpRuntime.AppDomainAppPath, "css");
            if (Directory.Exists(cssPath))
            	paths = paths.Concat(Directory.GetFiles(cssPath, "*.css"));
            foreach (string filePath in paths)
            {
                string css = Path.GetFileName(filePath);
                string relativePath = ("~\\" + filePath.Substring(HttpRuntime.AppDomainAppPath.Length));
                if (AllowTouchUIStylesheet(css))
                	if (!(css.EndsWith(".min.css")))
                    	stylesheets.Add(relativePath);
                    else
                    {
                        int index = stylesheets.IndexOf((css.Substring(0, (css.Length - 7)) + "css"));
                        if (index > -1)
                        	stylesheets[index] = relativePath;
                        else
                        	stylesheets.Add(relativePath);
                    }
            }
            return stylesheets;
        }
        
        private static string DoReplaceCssUrl(Match m)
        {
            string header = m.Groups["Header"].Value;
            string name = m.Groups["Name"].Value;
            string symbol = m.Groups["Symbol"].Value;
            if (((name == "data") || name.StartsWith("http")) && (symbol == ":"))
            	return m.Value;
            return (header 
                        + ("../touch/" 
                        + (name + symbol)));
        }
        
        public static string CombineTouchUIStylesheets(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpCachePolicy cache = response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByHeaders["User-Agent"] = true;
            cache.SetOmitVaryStar(true);
            cache.SetExpires(DateTime.Now.AddDays(365));
            cache.SetValidUntilExpires(true);
            cache.SetLastModifiedFromFileDependencies();
            // combine scripts
            string contentFramework = context.Request.QueryString["_cf"];
            bool includeBootstrap = (contentFramework == "bootstrap");
            StringBuilder sb = new StringBuilder();
            ApplicationServices services = Create();
            foreach (string stylesheet in services.EnumerateTouchUIStylesheets())
            {
                string cssName = Path.GetFileName(stylesheet);
                if (includeBootstrap || !(cssName.StartsWith("bootstrap")))
                	if (cssName.StartsWith("touch-theme."))
                    	sb.AppendLine(services.CompileThemeStylesheet(cssName));
                    else
                    {
                        string data = File.ReadAllText(HttpContext.Current.Server.MapPath(stylesheet));
                        data = CssUrlRegex.Replace(data, DoReplaceCssUrl);
                        if (!(data.Contains("@import url")))
                        	sb.AppendLine(data);
                        else
                        	sb.Insert(0, data);
                    }
            }
            return sb.ToString();
        }
        
        public virtual string CompileThemeStylesheet(string fileName)
        {
            string template = String.Empty;
            Match m = ThemeStylesheetRegex.Match(fileName);
            if (m.Success)
            {
                string touchPath = HttpContext.Current.Server.MapPath("~/touch");
                string theme = Path.Combine(touchPath, (("touch-theme." + m.Groups["Theme"].Value) 
                                + ".css"));
                string accent = Path.Combine(touchPath, (("touch-accent." + m.Groups["Accent"].Value) 
                                + ".json"));
                if (File.Exists(theme))
                {
                    template = File.ReadAllText(theme);
                    if (File.Exists(accent))
                    {
                        _accentJSON = JObject.Parse(File.ReadAllText(accent));
                        _themeVariables = new SortedDictionary<string, string>();
                        template = ThemeVariableRegex.Replace(template, DoReplaceThemeVariables);
                    }
                }
            }
            return template;
        }
        
        protected string DoReplaceThemeVariables(Match m)
        {
            string variable = m.Groups["Name"].Value;
            string before = m.Groups["Before"].Value;
            string after = m.Groups["After"].Value;
            string[] parts = variable.Split(',');
            string value = null;
            foreach (string part in parts)
            	if (TryGetThemeVariable(part.Trim().Substring(1), out value))
                	break;
            if (String.IsNullOrEmpty(value))
            	value = m.Groups["Value"].Value;
            if (EnableMinifiedCss)
            	return ((before + value) 
                            + after);
            else
            	return ((before 
                            + (" /*" + variable)) 
                            + (("*/ " + value) 
                            + after));
        }
        
        protected bool TryGetThemeVariable(string name, out string value)
        {
            if (!(_themeVariables.TryGetValue(name, out value)))
            {
                JToken token = TryGetJsonProperty(_accentJSON, name);
                if ((token != null) && token.Type != JTokenType.Null)
                {
                    value = ((string)(token));
                    _themeVariables[name] = value;
                }
            }
            return !(String.IsNullOrEmpty(value));
        }
        
        public virtual void ConfigureScripts(List<ScriptReference> scripts)
        {
            string jsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "js", "custom");
            if (Directory.Exists(jsPath))
            	foreach (string file in Directory.GetFiles(jsPath, "*.js"))
                	scripts.Add(AquariumExtenderBase.CreateScriptReference(("~\\" + file.Substring(HttpRuntime.AppDomainAppPath.Length))));
        }
        
        public static void CompressOutput(HttpContext context, string data)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!(String.IsNullOrEmpty(acceptEncoding)))
            	if (acceptEncoding.Contains("gzip"))
                {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    response.AppendHeader("Content-Encoding", "gzip");
                }
                else
                	if (acceptEncoding.Contains("deflate"))
                    {
                        response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                        response.AppendHeader("Content-Encoding", "deflate");
                    }
            byte[] output = Encoding.UTF8.GetBytes(data);
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("Content-Length", output.Length.ToString());
            response.OutputStream.Write(output, 0, output.Length);
            try
            {
                response.Flush();
            }
            catch (Exception )
            {
            }
        }
        
        public static void HandleServiceRequest(HttpContext context)
        {
            byte[] data = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(data, 0, data.Length);
            string json = Encoding.UTF8.GetString(data);
            object result = null;
            string methodName = context.Request.Path.Substring((((Page)(context.Handler)).ResolveUrl(AquariumExtenderBase.DefaultServicePath).Length + 1));
            if ((String.IsNullOrEmpty(methodName) || (String.IsNullOrEmpty(json) && !(Regex.IsMatch(methodName, "^(Logout|Themes|ListAllPermalinks|Roles)$")))) || context.Request.HttpMethod != "POST")
            	context.Response.End();
            JObject args = null;
            if (!(String.IsNullOrEmpty(json)))
            	args = JObject.Parse(json);
            ServiceRequestHandler handler = null;
            if (RequestHandlers.TryGetValue(methodName.ToLower(), out handler))
            	try
                {
                    result = handler.HandleRequest(new DataControllerService(), args);
                }
                catch (Exception ex)
                {
                    result = handler.HandleException(args, ex);
                }
            if (result != null)
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                string output = String.Format("{{\"d\":{0}}}", JsonConvert.SerializeObject(result));
                ApplicationServices.CompressOutput(context, CompressViewPageJsonOutput(output));
            }
            context.Response.End();
        }
        
        public static string CompressViewPageJsonOutput(string output)
        {
            int startIndex = 0;
            int dataIndex = 0;
            int lastIndex = 0;
            int lastLength = output.Length;
            while (true)
            {
                startIndex = output.IndexOf("{\"Controller\":", lastIndex, StringComparison.Ordinal);
                dataIndex = output.IndexOf(",\"NewRow\":", lastIndex, StringComparison.Ordinal);
                if ((startIndex < 0) || (dataIndex < 0))
                	break;
                string metadata = (output.Substring(0, startIndex) + ViewPageCompressRegex.Replace(output.Substring(startIndex, (dataIndex - startIndex)), String.Empty));
                if (metadata.EndsWith(","))
                	metadata = metadata.Substring(0, (metadata.Length - 1));
                output = (ViewPageCompress2Regex.Replace(metadata, "}$1") + output.Substring(dataIndex));
                lastIndex = ((dataIndex + 10) 
                            - (lastLength - output.Length));
                lastLength = output.Length;
            }
            return output;
        }
        
        public virtual SortedDictionary<string, string> CorsConfiguration(HttpRequest request)
        {
            return null;
        }
        
        private static void EnsureJsonProperty(JObject ptr, string path, object defaultValue)
        {
            if (defaultValue == null)
            	defaultValue = String.Empty;
            string[] parts = path.Split('.');
            int counter = parts.Length;
            foreach (string part in parts)
            {
                counter--;
                if (ptr[part] == null)
                	if (counter != 0)
                    	ptr[part] = new JObject();
                    else
                    	ptr[part] = JToken.FromObject(defaultValue);
                if (counter != 0)
                	ptr = ((JObject)(ptr[part]));
            }
        }
        
        private static JToken TryGetJsonProperty(JObject ptr, string path)
        {
            string[] parts = path.Split('.');
            JToken temp = null;
            for (int i = 0; (i < (parts.Length - 1)); i++)
            {
                temp = ptr[parts[i]];
                if (temp != null)
                	ptr = ((JObject)(temp));
                else
                	return null;
            }
            return ptr[parts[(parts.Length - 1)]];
        }
    }
    
    public class AnonymousUserIdentity : IIdentity
    {
        
        string IIdentity.AuthenticationType
        {
            get
            {
                return "None";
            }
        }
        
        bool IIdentity.IsAuthenticated
        {
            get
            {
                return false;
            }
        }
        
        string IIdentity.Name
        {
            get
            {
                return String.Empty;
            }
        }
    }
    
    public partial class ApplicationSiteMapProvider : ApplicationSiteMapProviderBase
    {
    }
    
    public class ApplicationSiteMapProviderBase : System.Web.XmlSiteMapProvider
    {
        
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            string device = node["Device"];
            bool isTouchUI = ApplicationServices.IsTouchClient;
            if ((device == "touch") && !(isTouchUI))
            	return false;
            if ((device == "desktop") && isTouchUI)
            	return false;
            return base.IsAccessibleToUser(context, node);
        }
    }
    
    public partial class PlaceholderHandler : PlaceholderHandlerBase
    {
    }
    
    public class PlaceholderHandlerBase : IHttpHandler
    {
        
        private static Regex _imageSizeRegex = new Regex("((?\'background\'[a-zA-Z0-9]+?)-((?\'textcolor\'[a-zA-Z0-9]+?)-)?)?(?\'width\'[0-9]+?)(" +
                "x(?\'height\'[0-9]*))?\\.[a-zA-Z][a-zA-Z][a-zA-Z]");
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return true;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            // Get file name
            RouteValueDictionary routeValues = context.Request.RequestContext.RouteData.Values;
            string fileName = ((string)(routeValues["FileName"]));
            // Get extension
            string ext = Path.GetExtension(fileName);
            ImageFormat format = ImageFormat.Png;
            string contentType = "image/png";
            if (ext == ".jpg")
            {
                format = ImageFormat.Jpeg;
                contentType = "image/jpg";
            }
            else
            	if (ext == ".gif")
                {
                    format = ImageFormat.Gif;
                    contentType = "image/jpg";
                }
            // get width and height
            Match regexMatch = _imageSizeRegex.Matches(fileName)[0];
            Capture widthCapture = regexMatch.Groups["width"];
            int width = 500;
            if (widthCapture.Length != 0)
            	width = Convert.ToInt32(widthCapture.Value);
            if (width == 0)
            	width = 500;
            if (width > 4096)
            	width = 4096;
            Capture heightCapture = regexMatch.Groups["height"];
            int height = width;
            if (heightCapture.Length != 0)
            	height = Convert.ToInt32(heightCapture.Value);
            if (height == 0)
            	height = 500;
            if (height > 4096)
            	height = 4096;
            // Get background and text colors
            Color background = GetColor(regexMatch.Groups["background"], Color.LightGray);
            Color textColor = GetColor(regexMatch.Groups["textcolor"], Color.Black);
            int fontSize = ((width + height) 
                        / 50);
            if (fontSize < 10)
            	fontSize = 10;
            Font font = new Font(FontFamily.GenericSansSerif, fontSize);
            // Get text
            string text = context.Request.QueryString["text"];
            if (String.IsNullOrEmpty(text))
            	text = string.Format("{0} x {1}", width, height);
            // Get position for text
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                Graphics textDrawing = Graphics.FromImage(img);
                textSize = textDrawing.MeasureString(text, font);
            }
            // Draw the image
            using (Image image = new Bitmap(width, height))
            {
                Graphics drawing = Graphics.FromImage(image);
                drawing.Clear(background);
                using (Brush textBrush = new SolidBrush(textColor))
                	drawing.DrawString(text, font, textBrush, ((width - textSize.Width) 
                                    / 2), ((height - textSize.Height) 
                                    / 2));
                drawing.Save();
                drawing.Dispose();
                // Return image
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, format);
                    HttpCachePolicy cache = context.Response.Cache;
                    cache.SetCacheability(HttpCacheability.Public);
                    cache.SetOmitVaryStar(true);
                    cache.SetExpires(DateTime.Now.AddDays(365));
                    cache.SetValidUntilExpires(true);
                    cache.SetLastModifiedFromFileDependencies();
                    context.Response.ContentType = contentType;
                    context.Response.AddHeader("Content-Length", Convert.ToString(stream.Length));
                    context.Response.AddHeader("File-Name", fileName);
                    context.Response.BinaryWrite(stream.ToArray());
                    context.Response.OutputStream.Flush();
                }
            }
        }
        
        private static Color GetColor(Capture colorName, Color defaultColor)
        {
            try
            {
                if (colorName.Length > 0)
                {
                    string s = colorName.Value;
                    if (Regex.IsMatch(s, "^[0-9abcdef]{3,6}$"))
                    	s = ("#" + s);
                    return ColorTranslator.FromHtml(s);
                }
            }
            catch (Exception )
            {
            }
            return defaultColor;
        }
    }
    
    public class GenericRoute : IRouteHandler
    {
        
        private IHttpHandler _handler;
        
        public GenericRoute(IHttpHandler handler)
        {
            _handler = handler;
        }
        
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext context)
        {
            return _handler;
        }
        
        public static void Map(RouteCollection routes, IHttpHandler handler, string url)
        {
            Route r = new Route(url, new GenericRoute(handler));
            r.Defaults = new RouteValueDictionary();
            r.Constraints = new RouteValueDictionary();
            routes.Add(r);
        }
    }
    
    public class UserTicket
    {
        
        public string UserName;
        
        public string Email;
        
        public string Token;
        
        public string Picture;
        
        public UserTicket()
        {
        }
        
        public UserTicket(MembershipUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Picture = ApplicationServices.Create().UserPictureString(user);
        }
        
        public UserTicket(MembershipUser user, string token) : 
                this(user)
        {
            Token = token;
        }
    }
}
