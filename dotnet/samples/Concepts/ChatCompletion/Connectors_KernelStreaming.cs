﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ChatCompletion;

/// <summary>
/// This example shows how you can use Streaming with Kernel.
/// </summary>
/// <param name="output"></param>
public class Connectors_KernelStreaming(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunAsync()
    {
        string apiKey = TestConfiguration.AzureOpenAI.ApiKey;
        string chatDeploymentName = TestConfiguration.AzureOpenAI.ChatDeploymentName;
        string chatModelId = TestConfiguration.AzureOpenAI.ChatModelId;
        string endpoint = TestConfiguration.AzureOpenAI.Endpoint;

        if (apiKey == null || chatDeploymentName == null || chatModelId == null || endpoint == null)
        {
            WriteLine("Azure endpoint, apiKey, deploymentName or modelId not found. Skipping example.");
            return;
        }

        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: chatDeploymentName,
                endpoint: endpoint,
                serviceId: "AzureOpenAIChat",
                apiKey: apiKey,
                modelId: chatModelId)
            .Build();

        var funnyParagraphFunction = kernel.CreateFunctionFromPrompt("Write a funny paragraph about streaming", new OpenAIPromptExecutionSettings() { MaxTokens = 100, Temperature = 0.4, TopP = 1 });

        var roleDisplayed = false;

        WriteLine("\n===  Prompt Function - Streaming ===\n");

        string fullContent = string.Empty;
        // Streaming can be of any type depending on the underlying service the function is using.
        await foreach (var update in kernel.InvokeStreamingAsync<OpenAIStreamingChatMessageContent>(funnyParagraphFunction))
        {
            // You will be always able to know the type of the update by checking the Type property.
            if (!roleDisplayed && update.Role.HasValue)
            {
                WriteLine($"Role: {update.Role}");
                fullContent += $"Role: {update.Role}\n";
                roleDisplayed = true;
            }

            if (update.Content is { Length: > 0 })
            {
                fullContent += update.Content;
                Write(update.Content);
            }
        }

        WriteLine("\n------  Streamed Content ------\n");
        WriteLine(fullContent);
    }
}
