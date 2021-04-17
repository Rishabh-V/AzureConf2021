// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBotdemo.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //var replyText = $"Echo: {turnContext.Activity.Text}";
            //await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var replyText = new StringBuilder();
            var inputText = turnContext.Activity.Text.Trim();
            SearchClient client = GetSearchClient();
            var typingActivity = new Activity[] { new Activity { Type = ActivityTypes.Typing }, new Activity { Type = "delay", Value = 1000 } };
            await turnContext.SendActivitiesAsync(typingActivity);
            SearchResults<SearchDocument> response = await client.SearchAsync<SearchDocument>(inputText);

            await foreach (var result in response.GetResultsAsync())
            {
                replyText.AppendLine($"{result.Document["Path"].ToString()}");
            }

            string searchResult = replyText.ToString();

            if (string.IsNullOrWhiteSpace(searchResult))
            {
                searchResult = $"No match found";
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(searchResult), cancellationToken);
        }

        private static SearchClient GetSearchClient()
        {
            Uri serviceEndpoint = new Uri($"https://{Startup.SearchServiceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(Startup.QueryKey);
            SearchClient client = new SearchClient(serviceEndpoint, Startup.SearchIndexName, credential);
            return client;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome to Global Azure Conference 2021!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
