﻿using System.Collections.Generic;
using System.Linq;
using Bumblebee.Examples.Web.IntegrationTests.Shared;
using Bumblebee.Examples.Web.Pages.Reddit;
using Bumblebee.Extensions;
using Bumblebee.Setup;
using FluentAssertions;

using NUnit.Framework;

using OpenQA.Selenium;

namespace Bumblebee.Examples.Web.IntegrationTests
{
	// ReSharper disable InconsistentNaming

	[TestFixture]
	public class RedditTests
	{
		[SetUp]
		public void GoToReddit()
		{
			Threaded<Session>
				.With<HeadlessChrome>()
				.NavigateTo<LoggedOutPage>("https://old.reddit.com");
		}

		[TearDown]
		public void TearDown()
		{
			Threaded<Session>
				.End();
		}

		[Test]
		public void given_valid_credentials_when_logging_in_then_logged_in()
		{
			Threaded<Session>
				.CurrentBlock<LoggedOutPage>()
				.LoginArea
				.Email.EnterText("bumblebeeexample")
				.Password.EnterText("123abc!!")
				.LoginButton.Click()
				.VerifyPresenceOf("the logout link", By.CssSelector(".user a"));
		}

		[Test]
		public void given_invalid_credentials_when_loggging_in_then_return_user_to_login()
		{
			Threaded<Session>
				.CurrentBlock<LoggedOutPage>()
				.LoginArea
				.Email.EnterText("jjjjjjj")
				.Password.EnterText("jjjjjjj")
				.LoginButton.Click<LoggedOutPage>()
				.VerifyPresenceOf("the login area", By.Id("login_login-main"));
		}

        [Test]
		public void given_logged_out_when_at_front_page_then_posts_should_contain_til()
		{
			Threaded<Session>
				.CurrentBlock<LoggedOutPage>()
				.VerifyThat(page =>
					page.Posts.Any(post =>
						post.Subreddit.Text.Contains("todayilearned"))
						.Should().BeTrue("there should be at least one til on the front page"));
		}

		[Test]
		public void given_logged_out_when_at_front_page_then_posts_should_not_contain_selenium()
		{
			Threaded<Session>
				.CurrentBlock<LoggedOutPage>()
				.VerifyThat(page =>
					page.Posts.Any(post =>
						post.Subreddit.Text.Contains("selenium"))
						.Should().BeFalse("there should be no selenium subreddit posts on the front page"));
		}

		[Test]
		public void given_logged_out_at_front_page_when_clicking_random_featured_subreddit_first_page_should_start_with_1_and_second_page_should_start_with_26()
		{
		    var subreddits = Threaded<Session>
				.CurrentBlock<LoggedOutPage>()
				.FeaturedSubreddits;

		    var redditPage = subreddits
                .Store(out _, opt => opt.Count())
				.Take(2)
  				.Random()
			    .Click()
                .Store(out _, page => page.RankedPosts);

		    redditPage
                .DebugPrint(page =>
					page.RankedPosts
						.Select(post => post.Title.Text))
				.VerifyThat(page =>
					page.RankedPosts
						.First().Rank
						.Should().Be("1", "the first page should start with a rank of 1"))
				.Next.Click()
                .DebugPrint(page => 
			        page.RankedPosts.Count())
				.VerifyThat(page =>
					page.RankedPosts
						.First().Rank
						.Should().Be("26", "the second page should start with a rank of 26"));
		}
	}
}
