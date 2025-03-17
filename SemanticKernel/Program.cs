using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Plugins;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();


// Create a kernel builder
var builder = Kernel.CreateBuilder();

// Configure the builder with AI service (settings in appsettings.json)
builder.AddOpenAIChatCompletion(
    modelId: config["OpenAI:Model"], // Note the case and structure
    apiKey: config["OpenAI:ApiKey"],
    orgId: config["OpenAI:OrgId"] // Optional
);

// Plugins
builder.Plugins.AddFromType<NewsPlugin>();
builder.Plugins.AddFromType<ArchivePlugin>();

// Build the kernel
var kernel = builder.Build();

// Create a chat service
var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};


// Create a history to store the conversation
ChatHistory chatMessages = new ChatHistory("If the user doesn't provide a news category, ask for one." +
    "The following are some category suggestions that you can offer the user to use incase they didn't provide one:" +
    "World News, Technology, Education, Politics, Sports, Business, Science, and Health." +
    "If the user retrieves news, ask if they want to save it and get the file name. ");

while (true)
{
    Console.Write("Prompt: ");
    chatMessages.AddUserMessage(Console.ReadLine());

    var completion = chatService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel: kernel);

    string fullMessage = "";

    await foreach (var content in completion)
    {
        Console.Write(content.Content);
        fullMessage += content.Content;
    }

    chatMessages.AddAssistantMessage(fullMessage);
    Console.WriteLine();
}