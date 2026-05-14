scripts for [this](https://github.com/InverseThree/logicAppProj), don't steal w/o my permission

# patch notes
## V1.0.3 (2026 May 15)
### bug fixes<br/>
fix a bug where npc's statement text isn't changed in dialog when lamp is active<br/>
fix a bug where the popup panel about lamp being used up isn't showed when trying to use the lamp w/ no wish left
### behaviour change<br/>
now popup panel won't appear when clicking on an empty relic slot<br/>
now u can't select the item/relic anymore if it's the only chioce
### UI improvement<br/>
change the UI of popup panel when clicking on item/relic slot to show more useful texts and display current held item/relic sprite<br/>
now the statement list from answer panel is disabled when the panel is closed instead of always being active<br/>
now the truth table draft is on top of the answer panel instead of below it
### QOL improvement<br/>
now u can't waste ur lamp wish by choosing the same 2 npc when using the 2nd ability for the 2nd time on the same floor, or using the 3rd ability multiple times on the same floor
## V1.0.2 (2026 May 14)
### bug fixes<br/>
fix a bug where scythe can select the wrong npc indices when used from V1.0.1
## V1.0.1 (2026 May 14)
### bug fixes<br/>
fix a bug where gameover screen doesn't show when lens kills the player<br/>
fix a bug where clicking the main menu button in the gameClear or pause menu advances floor instead of how it should behave<br/>
### behaviour change<br/>
make sure shard rewards panel only shows up after floor 10 dialog was finished<br/>
add screenshake when taking damage<br/>
now u can't no longer select button using spacebar and enter (at least for the button in the minigame base UI)
### UI improvement<br/>
now unknown statements will be revealed when gameover<br/>
change scythe panel UI to match npcs' current position and show tooltip when hovering npcs' sprite<br/>
change tooltip description's position to make it look better<br/>
## V1.0 (2026 May 12)
### major update
add items and relics to the mini-game
#### items list
Tonic of God: Restore 1 health.<br/>
Shield of Ephemeral: The next time you are about to lose health on this floor, prevent it.<br/>
Lens of Devil: Lose 1 health to reveal a random inhabitant's identity.<br/>
Scroll of Insight: Reveal the number of either knights or knaves on this floor, but you don't know which.<br/>
Mirror of Truth: Reveal which 2 random inhabitants have the same identity on the next floor.<br/>
Hourglass of Infinity: Reset the state of this floor. (inhabitants might have different identities and statements!)
#### relics list
Coin of Polarity: Upon pick up, add 1 additional inhabitant for future floors. When your health reaches 0 for the first time, revive into full health instead.<br/>
Brush of Chaos: Upon pick up, add 5 additional floors after floor 10. Future floors have random number of inhabitants instead of set number.<br/>
Talisman of Sin: Upon pick up, set your max health to 1. The first time you are about to lose health on a floor, prevent it.<br/>
Shard of Falsehood: Upon pick up, you now lose 1 health for each incorrect guess in your submission instead. At the start of each floor, choose 1 out of 3 random items to get.<br/>
Scythe of Origination: Upon pick up, lose 1 item slot. Once per floor, you may kill any number of inhabitants to create new ones in their places. (with potentially different identities and statements!)<br/>
Lamp of Oracle: Upon pick up, hide the names of other inhabitants mentioned in a random inhabitant's statement for future floors. You now have 3 chances in total to make 1 of the following effects come true:<br/>
Reveal the identity of an inhabitant of your choice on this floor (can only be wished once).<br/>
Reveal whether the identity of 2 inhabitants of your choice is the same or not on this floor (can only be wished twice).<br/>
Reveal the number of knights and knaves on this floor (can be wished thrice).
## V0.1 (2026 Apr 29)
### 1st commit
1st ver. of the proj
#### statements pool for the mini-game (npcs can't refer themselves as kanve)
##### easy pool
\* is/is not a knight/knave.
##### medium pool
I am a knight or * is a knight/knave.<br/>
Both * and * are knight/knave.<br/>
It is not the case that both * and * are knight/knave.<br/>
\* and * have the same/different role.
##### hard pool
Only a knight/knave would say * is a knight/knave.<br/>
Either * is a knight/knave or * is a knight/knave, but not both.<br/>
Neither * nor * is a knight/knave.<br/>
Exactly one of us is a knight/knave.
#### statements spawn percentage per floor
floor 1-3: 70% easy statement, 20% medium statement, 10% hard statement<br/>
floor 4-6: 30% easy statement, 50% medium statement, 20% hard statement<br/>
floor 7-9: 20% easy statement, 40% medium statement, 40% hard statement
floor 10 onward: 10% easy statement, 35% medium statement, 55% hard statement
