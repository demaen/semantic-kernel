﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ChatCompletion;

// The following example shows how to use Semantic Kernel with OpenAI ChatGPT API
public class OpenAI_ChatCompletion(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task OpenAIChatSampleAsync()
    {
        WriteLine("======== Open AI - ChatGPT ========");

        OpenAIChatCompletionService chatCompletionService = new(TestConfiguration.OpenAI.ChatModelId, TestConfiguration.OpenAI.ApiKey);

        await StartChatAsync(chatCompletionService);

        /* Output:

        Chat content:
        ------------------------
        System: You are a librarian, expert about books
        ------------------------
        User: Hi, I'm looking for book suggestions
        ------------------------
        Assistant: Sure, I'd be happy to help! What kind of books are you interested in? Fiction or non-fiction? Any particular genre?
        ------------------------
        User: I love history and philosophy, I'd like to learn something new about Greece, any suggestion?
        ------------------------
        Assistant: Great! For history and philosophy books about Greece, here are a few suggestions:

        1. "The Greeks" by H.D.F. Kitto - This is a classic book that provides an overview of ancient Greek history and culture, including their philosophy, literature, and art.

        2. "The Republic" by Plato - This is one of the most famous works of philosophy in the Western world, and it explores the nature of justice and the ideal society.

        3. "The Peloponnesian War" by Thucydides - This is a detailed account of the war between Athens and Sparta in the 5th century BCE, and it provides insight into the political and military strategies of the time.

        4. "The Iliad" by Homer - This epic poem tells the story of the Trojan War and is considered one of the greatest works of literature in the Western canon.

        5. "The Histories" by Herodotus - This is a comprehensive account of the Persian Wars and provides a wealth of information about ancient Greek culture and society.

        I hope these suggestions are helpful!
        ------------------------
        */
    }

    [Fact]
    public async Task AzureOpenAIChatSampleAsync()
    {
        WriteLine("======== Azure Open AI - ChatGPT ========");

        AzureOpenAIChatCompletionService chatCompletionService = new(
            deploymentName: TestConfiguration.AzureOpenAI.ChatDeploymentName,
            endpoint: TestConfiguration.AzureOpenAI.Endpoint,
            apiKey: TestConfiguration.AzureOpenAI.ApiKey,
            modelId: TestConfiguration.AzureOpenAI.ChatModelId);

        await StartChatAsync(chatCompletionService);
    }

    private async Task StartChatAsync(IChatCompletionService chatGPT)
    {
        WriteLine("Chat content:");
        WriteLine("------------------------");

        var chatHistory = new ChatHistory("You are a librarian, expert about books");

        // First user message
        chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
        await MessageOutputAsync(chatHistory);

        // First bot assistant message
        var reply = await chatGPT.GetChatMessageContentAsync(chatHistory);
        chatHistory.Add(reply);
        await MessageOutputAsync(chatHistory);

        // Second user message
        chatHistory.AddUserMessage("I love history and philosophy, I'd like to learn something new about Greece, any suggestion");
        await MessageOutputAsync(chatHistory);

        // Second bot assistant message
        reply = await chatGPT.GetChatMessageContentAsync(chatHistory);
        chatHistory.Add(reply);
        await MessageOutputAsync(chatHistory);
    }

    /// <summary>
    /// Outputs the last message of the chat history
    /// </summary>
    private Task MessageOutputAsync(ChatHistory chatHistory)
    {
        var message = chatHistory.Last();

        WriteLine($"{message.Role}: {message.Content}");
        WriteLine("------------------------");

        return Task.CompletedTask;
    }
}
