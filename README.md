# hvmbot
This service downloads music & photo from any VKontakte community wall (you need to be an admin or owner of group) and resends it to your Telegram Channel.

Change ENV Vars in https://github.com/rhiskey/hvmbot-Public/blob/master/hvmbot/Configuration.cs
You need to set:
- "BotToken ": "", - from @BotFather
- "VK_LOGIN": "", - Community Admin
- "VK_PASS": "",
- "VK_GROUP_ID": "", - Community ID
- "AccessToken": "", - from @BotFather
- "tg_chat_id ": "", - Your Public Channel or Chat
- "VK_API_TOKEN": "" - https://vkhost.github.io/
