# ecs-messages

## Overview
This messaging system for DOTS implementation of ECS solves some problems of messaging.<br/>
It can be used as bridge between MonoBehavior based logic and ECS based logic or interaction service for ECS systems.<br/>

Key features:
- Simple API
- Handling messages lifetime(creation details, auto deleting by according to configured rules, etc)
- Support of any *IComponentData* as message content

## Use Cases

### UI and ECS 
There is a lot of reasons to implement UI logic via *Object Oriented Design*.<br/>
So we need somehow connect our ECS gameplay parts and interface elements.<br/>
For example communication between UI layer(buttons, swipe gesture) and ECS gameplay logic(start match by click, pause game).<br/>

![UI magic](documentation/use_case_ui.png)

### ECS Gameplay logic with Non-Gameplay logic 
It also OK for communication between ECS Systems without carying about enteties-messages creation and deleting.<br/>
Or classic architecture aproach interaction with high performance parts implemented with ECS pattern.<br/>
As example we can talk about achivements. Our player suddendly met "Game Over" window but game designer wants to give you achivement as reward. Naive people...<br/>
So *CharacterDeathSystem* just post message that available only for **one frame** via service API and hopes that *AchievementsListenerSystem* will react somehow to this sad news.<br/>

![Achiements happens..](documentation/use_case_achievement.png)

## Idea
In Data Oriented Design we can say that commands and events are enteties with bunch of special components.<br/>
So, from computer point of view they looks almost identicaly but not for developer.<br/>
Both are messages but with different semantic.<br/>
The difference between them in reasons why they were sent to world.<br/>
Event notifies that owner of this event **changed its own state**.<br/>
Command, despite they also just an entity with some components, **have intention to change someones state**.<br/>

![Everything is message](documentation/data_driven_message.png)

> Event - entity with bunch of components that notifies world about owner changed state.<br/> 
> Command - entity with bunch of components that have intetion to change someones state.<br/>

In classic OOP paradigm command is a peace of logic that have form of object. But in Data Driven Design we can operate only with data.<br/>

## Code Examples
```csharp
// Post a message-command to start game with medium difficulty settings. It will be available only for one frame and will be deleted.
MessageBroadcaster
    .PrepareCommand()
    .Post(new StartMatchData { Value = Difficulty.Medium } );

// Post a message-event that stun effect is active. It will be existing for next seven seconds and then would be deleted.
MessageBroadcaster
    .PrepareEvent()
    .WithLifeTime(7f)
    .Post(new StunEffectData());

// Post a message-event that NPC required by quest is killed. It still can be manually deleted and serve as "global flag with some data".
MessageBroadcaster
    .PrepareEvent()
    .WithUnlimitedLifeTime()
    .Post(new QuestNPCWasKilledData { NPC = NPC.TolikMerchant, Tick = 123456789 } );
```

## Editor Debug Tools
*WIP*