# Option Details Page

## AI Chance

Since events are not only available to players, the AI will sometimes have to choose between event options. To this end, you can configure the AI chance. While Paradox scripting allows you to create complex AI chance modifiers, Paradoxical™ simplifies this process: Each option defines a base chance, which can be further fine-tuned according to the AI's personality values. There are nine of these: boldness, compassion, greed, energy, honor, rationality, sociability, vengefulness, and zeal. These values can range from -100 to +100, with ±75 being considered a very high value.

The CK3 wiki provides [further reading](https://ck3.paradoxwikis.com/Character#AI_Personality) if you are interested in exactly what these personality values stand for, and how they influence AI behaviour in general.

For the purposes of Paradoxical™, you only need to know that the base AI chance for event options can be further modified by special AI target modifiers, of which there is one for every personality value. You can define a modifier value for each personality value, which will be weighed by it, before being added to the option's base AI chance.

The weighing process simply divides the AI's personality value, by what Paradox considers to be a high personality value (as of the time of writing this, that would be 75), then multiplies this factor with the specified modifier value. So if you specify a boldness AI chance modifier of 40, you can expect 40 to be added to the base chance, if the AI has a boldness value of 75; or you can expect 8 to be added, if the AI has a boldness value of 15 (15÷75×40).
