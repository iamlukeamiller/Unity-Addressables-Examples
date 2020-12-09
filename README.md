# Unity Addressables Examples
After some weeks of trying, failing, and trying again to get the Unity Addressables system working using standard coroutines, I finally got my own system working in a way that works for my application.

In the interest of community, and knowing just how frustrating that the Addressables system can be to wrap your head around, I wanted to post some simple examples of my own implementation in the hopes that it will help other developers pick this up more quickly.

## Disclaimer
I 100% am relatively new to Unity (started in the last year), and do NOT claim to know all best practices and ideal implementation for things like coroutines, addressables, etc. My examples here are simply pulled from my own code, and I guarantee there are skilled developers out there that would write these in different ways.

Take all of this code with a grain of salt, and don't expect peak performance, flexibility or best practices here.

## Credit
I want to give a HUGE thank you to BadgerDox and [his YouTube channel](https://www.youtube.com/channel/UCoLsrnLGqMy2yxzICwwH0MQ). While I didn't end up using the async/await technique his videos show, I gained a TON of context from watching him use Addressables and then playing with the results myself.
