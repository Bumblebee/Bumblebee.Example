﻿using Bumblebee.Implementation;
using Bumblebee.Interfaces;
using Bumblebee.Setup;

using OpenQA.Selenium;

namespace Bumblebee.Examples.Web.Pages.Nirvana
{
	public class ToolBar : WebBlock
	{
		public ToolBar(Session session) : base(session)
		{
			Tag = FindElement(By.Id("north"));
		}

		public ITextField<ToolBar> SearchField => new TextField<ToolBar>(this, By.ClassName("q"));

	    public IClickable<NewTaskForm> NewTask => new Clickable<NewTaskForm>(this, By.ClassName("newtask"));

	    public IHasText Account => new TextField(this, By.CssSelector("a.right.button.accountmenu.xcmenu"));
	}
}
