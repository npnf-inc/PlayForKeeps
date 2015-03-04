# PlayForKeeps
This game showcases how to use the features of npnf Platform. Build your next game faster with the npnf Platform.

Download the latest SDK here: https://developer.npnf.com/download

## Game Logic
You are an intern at our office, and today is your first day at work. You have six minutes to talk to every new colleague and collect the items required to craft a pitch for our CEO.

Collect all the items in a category, and then fuse them into better items: strength, morale, and funding. Then fuse strength, morale, and funding into a pitch. To win the game, take the pitch to Woo, our CEO.

With every conversation, your colleague will give you something. You can talk to each colleague a maximum of three times.

In order to talk to a colleague, you must have enough energy. Each conversation costs 20 units of energy. The maximum amount of energy is 180 units. Energy is automatically recharged at a rate of 3 units/second. You can also buy energy: 5 coins gets you 8 units of energy.

To see how the game looks, you play the web version here:

## Controls
Use the following keys to control the main character:
<dl>
  <dt>ARROW KEYS</dt>
  <dd>Move around.</dd>
  
  <dt>SPACE BAR</dt>
  <dd>Start a conversation with a colleague.</dd>
  
  <dt>R</dt>
  <dd>Recharge energy. It costs 5 coins to get 8 units of energy.</dd>
  
  <dt>F</dt>
  <dd>Fuse items together to make something better.</dd>
</dl>

## npnf Platform Features
This game showcases our major gaming modules:

* Gacha: Configured as a loot table to randomly award an item when you have a conversation with a colleague. Newly acquired items are automatically added to your inventory of collected items.

* Energy: You get an energy bar with 180 units of energy. Energy is recharged at a rate of three units per second. The game logic deducts 20 units of energy every time you talk wiht a colleague.

* Currency: Virtual currency is used to buy more energy. Ten virtual coins are added your currency balance when a colleagues gives you a duplicate item. You can use currency to purchase more energy. Five coins buys eight units of energy.

* Collections: There are a total of thirteen objects that you can collect and that can be added your inventory. The goal is to collect them and fuse them until you get to your end goal: a pitch.

* Fusion: Recipes let you evolve basic items into more powerful items. After you collect all items in a category, execute a fusion. The npnf platform checks that you've got all the prerquisutes, and then executes the fusion formula, deducting any items from your inventory and adding any new items.

## Game End
__WIN__:
Get your pitch by fusing together all the basic items you've collected, and then take it to Woo (our CEO) before the six minute timer expires.

__LOSE__:
The timer expires, and you still don't have a pitch.

## Code
The first scene is in Assets/PlayForKeeps/Scenes/StartUp.unity.

The main folders in the project are:

* __AppComponents__ - Scripts that synchronize the game with the configurations set up in the Developer Portal.

* __SceneComponents__ - Controller scripts, which demo how to use platform features, such as Collections, Currency, Energy, Gacha, and Fusion.

* __SharedComponents__ - Main script, AppController, which is a singleton.

## Developer Portal Configurations
The game depends on a number of configurations in the Developer Portal. We will provide an account that has a read-only version of these configurations shortly.

In the meantime, if you want a copy of those configurations, email support@npnf.com.

## Running the Game
This game was designed to run in stand-alone mode only. This is not a limitation of the SDK, but of the game itself.

Note: We will put up a playable version of this game on https://developer.npnf.com/docs shortly.
