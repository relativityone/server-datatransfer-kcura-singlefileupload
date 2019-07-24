﻿using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.MVC.Models;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using Relativity.API;
using Relativity.CustomPages;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace kCura.SingleFileUpload.MVC.Controllers
{
	/// <summary>
	/// This is like an infrastructure class... please make all your controllers inherit of this class.
	/// </summary>
	public class BaseController : Controller
	{

		/*
		 If you have different specific definitions like repos or classes and are traverse for the 
		 application please add here... plase, avoid repeated code
		 */

		private CacheContextScope _scopeDictionary;
		private IDocumentManager __repositoryDocumentManager;
		private int _workspaceID;
		private IUserInfo _relativityUserInfo;

		/// <summary>
		/// Current WorkspaceID
		/// </summary>
		private const string _APP_ID = "AppID";
		protected int WorkspaceID
		{
			get
			{
				if (_workspaceID == 0)
				{
					if (Request.Params.AllKeys.Any(p => string.Compare(p, _APP_ID, true) == 0))
					{
						_workspaceID = int.Parse(Request.Params[_APP_ID]);
					}
					else
					{
						if (Request.Headers.AllKeys.Any(p => string.Compare(p, _APP_ID, true) == 0))
						{
							_workspaceID = int.Parse(Request.Headers[_APP_ID]);
						}
						else
						{
							if (RefererAppId.HasValue)
							{
								_workspaceID = RefererAppId.Value;
							}
							else
							{
								throw new ApplicationException("AppID is required.");
							}

						}
					}
				}
				return _workspaceID;
			}
		}

		protected IDocumentManager _RepositoryDocumentManager
		{
			get
			{
				if (__repositoryDocumentManager == null)
				{
					__repositoryDocumentManager = new DocumentManager();
				}
				return __repositoryDocumentManager;
			}
		}

		private int? RefererAppId
		{
			get
			{
				var reg = new System.Text.RegularExpressions.Regex("^\\?AppID=(?<AppID>[\\d]+)");
				Match match = reg.Match(Request.UrlReferrer.Query);
				if (match.Success)
				{
					string appIdAsString = match.Groups["AppID"].Value;
					if (int.TryParse(appIdAsString, out int appId))
					{
						return appId;
					}
				}
				return null;
			}
		}
		/// <summary>
		/// Current User logged in relativity info, please use it
		/// </summary>
		protected IUserInfo RelativityUserInfo
		{
			get
			{
				if (_relativityUserInfo == null)
				{
					_relativityUserInfo = ConnectionHelper.Helper().GetAuthenticationManager().UserInfo;
				}
				return _relativityUserInfo;
			}
		}
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			_scopeDictionary = RepositoryHelper.InitializeRepository(WorkspaceID);
			base.OnActionExecuting(filterContext);
		}
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
			_scopeDictionary.Dispose();
		}

		/// <summary>
		/// Returns a default response with data of type T
		/// </summary>
		/// <typeparam name="T">type of the data to be returned</typeparam>
		/// <param name="lambda">function to be executed in a controlled context</param>
		/// <returns></returns>
		protected internal ResponseWithElements<T> HandleResponse<T>(Func<ResponseWithElements<T>, T> lambda)
		{
			ResponseWithElements<T> response = new ResponseWithElements<T>();
			try
			{
				response.Success = true;
				response.Data = lambda.Invoke(response);
			}
			catch (Exception e)
			{
				response.Message = LogException(e);
				response.Success = false;
			}
			return response;
		}
		protected internal async Task<ResponseWithElements<T>> HandleResponseDynamicResponseAsync<T>(Func<ResponseWithElements<T>, Task<T>> lambda)
		{
			ResponseWithElements<T> response = new ResponseWithElements<T>();
			try
			{
				response.Data = await lambda.Invoke(response);
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Message = LogException(e);
				response.Success = false;
			}
			return response;
		}
		protected internal async Task<ResponseWithElements<T>> HandleResponseAsync<T>(Func<Task<T>> lambda)
		{
			ResponseWithElements<T> response = new ResponseWithElements<T>();
			try
			{
				response.Data = await lambda.Invoke();
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Message = LogException(e);
				response.Success = false;
			}
			return response;
		}
		protected internal async Task<Response> HandleResponseAsync(Func<Task> lambda)
		{
			Response response = new Response();
			try
			{
				await lambda.Invoke();
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Message = LogException(e);
				response.Success = false;
			}
			return response;
		}
		private string LogException(Exception e)
		{
			string errorMessage = e.Message;
			_RepositoryDocumentManager.LogError(e);
			return $"{errorMessage.Replace("Error:", string.Empty).Replace("\r\n", string.Empty).Replace("'", string.Empty)}";
		}
		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			try
			{
				if (Request.HttpMethod == "POST")
				{
					ConnectionHelper.Helper().GetCSRFManager().CheckCSRF();
				}
			}
			catch (Exception)
			{
				filterContext.Result = new HttpUnauthorizedResult();
			}
			base.OnAuthorization(filterContext);
		}
	}
}
