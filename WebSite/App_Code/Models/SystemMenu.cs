using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class SystemMenuModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pageUrl;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pageDescription;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _createBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _createDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _updateBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _updateDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pageRoles;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pageName;
        
        public SystemMenuModel()
        {
        }
        
        public SystemMenuModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public string PageUrl
        {
            get
            {
                return _pageUrl;
            }
            set
            {
                _pageUrl = value;
                UpdateFieldValue("PageUrl", value);
            }
        }
        
        public string PageDescription
        {
            get
            {
                return _pageDescription;
            }
            set
            {
                _pageDescription = value;
                UpdateFieldValue("PageDescription", value);
            }
        }
        
        public string CreateBy
        {
            get
            {
                return _createBy;
            }
            set
            {
                _createBy = value;
                UpdateFieldValue("CreateBy", value);
            }
        }
        
        public DateTime? CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
                UpdateFieldValue("CreateDate", value);
            }
        }
        
        public string UpdateBy
        {
            get
            {
                return _updateBy;
            }
            set
            {
                _updateBy = value;
                UpdateFieldValue("UpdateBy", value);
            }
        }
        
        public DateTime? UpdateDate
        {
            get
            {
                return _updateDate;
            }
            set
            {
                _updateDate = value;
                UpdateFieldValue("UpdateDate", value);
            }
        }
        
        public string PageRoles
        {
            get
            {
                return _pageRoles;
            }
            set
            {
                _pageRoles = value;
                UpdateFieldValue("PageRoles", value);
            }
        }
        
        public string PageName
        {
            get
            {
                return _pageName;
            }
            set
            {
                _pageName = value;
                UpdateFieldValue("PageName", value);
            }
        }
    }
}
