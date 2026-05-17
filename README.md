scripts for [this](https://github.com/InverseThree/logicAppProj), don't steal w/o my permission

# patch notes
## V1.1 (2026 May 17)
### major update<br/>
#### summary
add hints for the tutorial chapters so it's friendlier for beginner to pass the quizzes. also adjust the statement pool with new statements and refined statements' difficulty level (plus other miscellaneous changes)
#### new statements pool for the mini-game (npcs can't refer themselves as kanve)
##### easy pool
P's/isn't a knight/knave.<br/>
I'm/P's a knight/knave or Q's a knight/knave.<br/>
Both I/P and Q are knight/knave.<br/>
It is not the case that both I/P and Q are knight/knave.<br/>
Exactly one of us is a knight/knave.
##### medium pool
Exactly two of us are knights/knaves. (not available for floors w/ only 2 npcs)<br/>
Only a knight/knave would say P's a knight/knave.<br/>
Either I'm/P's a knight/knave or Q's a knight/knave, but not both.<br/>
Neither I/P nor Q is a knight/knave.<br/>
I/P and Q have the same/different role.<br/>
P's a knight/knave when Q's a knight/knave.
##### hard pool
Exactly three of us are knights/knaves. (not available for floors w/ less than 5 npcs)<br/>
P could say Q's a knight/knave.<br/>
P's a knight/knave unless Q's a knight/knave.<br/>
P's a knight/knave when Q's a knight/knave, and vice versa.<br/>
P's a knight/knave unless Q's a knight/knave, and vice versa.<br/>
P being a knight/knave is necessary/sufficient but not sufficient/necessary for Q to be a knight/knave.<br/>
P being a knight/knave is both necessary and sufficient for Q to be a knight/knave.
##### dynamic difficulty
More/less than half of us are knights/knaves. (in easy/medium/hard pool for floors w/ 2/3/4+ npcs)
#### bug fixes<br/>
fix a bug where clicking the answerpanel toggle button doesn't show the statement list from V1.0.3<br/>
fix a bug where u can still click on the relic slot or item slot when dialog is playing<br/>
fix a bug where the tutorial for the minigame would play again when u chose the 1st option during ur conversation w/ Conti on floor 1
#### behaviour change<br/>
now u can no longer see an item that is the same as ur current held item<br/>
now u can no longer get tonic when u have talisman<br/>
now u can no longer get lens if u only have 1 health left w/o having talisman<br/>
now u can no long get mirror as ur floor 9 item reward unless u have brush<br/>
make item actually drops every 3 floor instead of only on floor 3/6/9<br/>
nerf tonic and mirror so that u can't use them on the floor u obtained them<br/>
nerf brush to actually spawn random no. of npcs instead of capping at the base npcs no. of the floor
#### UI improvement<br/>
now the tooltip will no longer be showed when hovering the relic/item sprite from ur slot while dialog is playing<br/>
now the relic/item sprite from ur slot will fade along w/ the slot when dialog is playing<br/>
now ur item slot will fade when u have scythe<br/>
refine the UI for the run info bar, answer panel, and slots
#### other changes<br/>
update the tutorial for the chapters<br/>
now players can open the connectives list and truth tables list for each connectives in the tutorial chapters<br/>
update the tutorial for the minigame<br/>
fix the grammar for the "neither… nor…" statements<br/>
add an extra space after every punctuation for in-game sentences since it looked like there's no space after the punctuation when there's only 1 space due to the font<br/>
add 2 extra variants about disjunction for the IsKnave statement<br/>
change statement type 'ExactlyOneIsKnight' and statement type 'ExactlyOneIsKnave' to 'ExactlyXAreKnights' and 'ExactlyXAreKnaves'<br/>
add statement type: 'HalfAreKnights', 'HalfAreKnaves', 'CouldSayKnight', 'CouldSayKnight', 'KnightWhenKnight', 'KnightWhenKnave', 'KnaveWhenKnight', 'KnaveWhenKnave', 'KnightNecessaryForKnight', 'KnightNecessaryForKnave', 'KnaveNecessaryForKnight', and 'KnaveNecessaryForKnave'

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
P's/isn't a knight/knave.
##### medium pool
I'm a knight or P's a knight/knave.<br/>
Both I/P and Q are knight/knave.<br/>
It is not the case that both I/P and Q are knight/knave.<br/>
I/P and Q have the same/different role.
##### hard pool
Only a knight/knave would say P's a knight/knave.<br/>
Either I'm/P's a knight/knave or Q's a knight/knave, but not both.<br/>
Neither I/P nor Q is a knight/knave.<br/>
Exactly one of us is a knight/knave.
#### statements spawn percentage per floor
floor 1-3: 70% easy statement, 20% medium statement, 10% hard statement<br/>
floor 4-6: 30% easy statement, 50% medium statement, 20% hard statement<br/>
floor 7-9: 20% easy statement, 40% medium statement, 40% hard statement<br/>
floor 10 onward: 10% easy statement, 35% medium statement, 55% hard statement
