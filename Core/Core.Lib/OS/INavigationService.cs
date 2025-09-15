using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Lib.OS
{
	interface INavigationService
	{
		/// <summary>Navigate to previous view</summary>
		/// <returns></returns>
		Task GoBack();
		/// <summary>Clean a navigation stack to the root</summary>
		/// <returns></returns>
		Task Home();
		/// <summary>Used to remove a modal</summary>
		/// <returns></returns>
		Task PopModal();
		/// <summary>Show a modal</summary>
		/// <param name="pageKey"></param>
		/// <returns></returns>
		Task PushModal(string pageKey);
		/// <summary>Navigate to a page without the page requiring any parameters</summary>
		/// <param name="pageKey"></param>
		/// <returns></returns>
		Task NavigateTo(string pageKey);
		/// <summary>Navigate to a page with parameters</summary>
		/// <param name="pageKey"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		Task NavigateTo(string pageKey, params object[] parameter);

		void NavigatePop();

		void NavigateToUrl(string url);
	}
}
