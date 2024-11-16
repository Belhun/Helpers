New Pawn Atrabutes and the logic am thinking about useing:
- (in dont know if this should be a list or a Coil?)'CurrentHelpers' - these are all the pawns helping the Crafting pawns
- bool 'IsBeingHelped' - This is how we check if the pawn whos crafting is being helped by anyone. 

When a pawn starts to help a crafter then we turn 'IsBeingHelped' to true and we add the pawn to the crafters 'CurrentHelpers'

Anytime a helper stops helping a crafting pawn then, we need to go to 'CurrentHelpers' find the helper that leaveing(aka that stops helping this pawn) and remove them from that list, then check if anyone else is left in the list and if no one is then we turn 'IsBeingHelped' to false.


when they Calculate Work Speed for the job, we need to change the calulation to add a check for weather or not the Crafting pawn's 'IsBeingHelped' 
is true and if so then we grab the crafter pawns CurrentHelpers, and on each pawn we do `Helper Contribution = (0.5 + Helper Level/40) × Helpers work speed` and add that to the 'HelpersTotal' var then after going thru each helper we grab the 'HelpersTotal' and add that too the main crafters workSpeedStat, then continue on with the normal calculations.

When we get too give exsperence too the Crafters Skill we do a Simular check on weather or not the crafting pawn 'IsBeingHelped', and if so we take go thru each `CurrentHelpers` and add the same amount of exsperence to everyone. So if the Main Crafter would get 0.5 exsperence in there currently being used crafting skill, then every helper would get the same amount in there currently being used crafting skill. on top of the we will also do the same amount of exsperence to the Helping skill for all the Helpers and Helped Pawns.

Helper Trade-offs:


Balancing your mod to ensure that having multiple helpers doesn't feel overpowered or punitive can be approached creatively without relying on a harsh mechanic like food poisoning. Here are some alternative balancing methods for scenarios where multiple pawns are helping with tasks like cooking or crafting:

---

### **Efficiency Penalty: "Too Many Cooks" Mechanic**
- **Concept**: As the number of helpers increases, efficiency gains diminish. 
- **Implementation**:
  - Introduce a diminishing returns formula. For example:
    \[
    \text{EffectiveHelperContribution} = \text{BaseContribution} \times \frac{1}{1 + (\text{HelpersCount} \times \text{DiminishingFactor})}
    \]
    - HelpersCount: Number of helpers.
    - DiminishingFactor: Tunable value, e.g., 0.25.
  - Each additional helper contributes less to the overall work speed.
- **Effect**: Encourages strategic use of helpers without penalizing the player too harshly.

---

### **Quality Penalty for Products**
- **Concept**: Adding helpers increases speed but reduces the final quality of the output.
- **Implementation**:
  - Scale the maximum quality of crafted items based on the number of helpers:
    \[
    \text{MaxQuality} = \text{BaseMaxQuality} - (\text{HelpersCount} \times \text{QualityPenalty})
    \]
    - QualityPenalty: A small percentage (e.g., 5–10% per helper).
  - Limit penalties to avoid complete devaluation.
- **Effect**: Simulates the idea of "rushed work" leading to poorer results, balancing speed with quality.

---

### **Increased Resource Usage**
- **Concept**: More helpers working together can lead to inefficiency in material usage.
- **Implementation**:
  - Introduce a small chance for "wasted materials" for each helper:
    - Example: Each helper has a 5% chance to consume 10% more of a resource per crafting iteration.
  - Apply this primarily to crafting jobs, not cooking, to make it less punitive for survival-critical tasks.
- **Effect**: Adds a subtle penalty without frustrating the player.

---

### **Increased Energy or Rest Drain**
- **Concept**: Assisting in crafting or cooking is mentally and physically taxing for helpers.
- **Implementation**:
  - Increase the rate at which helpers lose rest or recreation needs while assisting.
  - Optionally, apply this penalty only if more than one helper is involved.
- **Effect**: Makes excessive reliance on helpers less viable over long periods.

---

### **Increased Chance of Mistakes**
- **Concept**: Crowded workspaces or uncoordinated teamwork lead to minor mistakes.
- **Implementation**:
  - Add a small chance for:
    - Failed crafting attempts (wasting materials).
    - Spoiled food batches (if cooking).
  - Scale the chance based on the number of helpers.
- **Effect**: Provides a natural balancing factor without overly punishing small helper groups.

---

### **Helper Skill Dependency**
- **Concept**: Helpers need to have sufficient skill to contribute effectively.
- **Implementation**:
  - Require a minimum level in the relevant skill (e.g., cooking, crafting) for helpers to provide meaningful contributions.
  - Helpers below the threshold contribute minimal or negative efficiency (e.g., slowing down the main worker slightly).
- **Effect**: Encourages thoughtful selection of helpers and discourages using unskilled pawns indiscriminately.

---

### **Social Interaction Drawbacks**
- **Concept**: Crowded workspaces can lead to social tensions or distractions.
- **Implementation**:
  - Add a small negative moodlet if too many pawns are working in close proximity (e.g., "Crowded Workspace").
  - Scale the penalty based on the number of helpers or their relationship compatibility with the main worker.
- **Effect**: Adds realism and forces players to manage their colony dynamics more carefully.

---

### **Helper Dependency Progression**
- **Concept**: Balancing mechanics can scale with progression to avoid early-game frustration.
- **Implementation**:
  - Early game:
    - Helpers contribute more effectively without significant penalties.
  - Late game:
    - Introduce penalties like diminishing returns, increased resource usage, or moodlets.
- **Effect**: Ensures that the mod feels helpful early on but balanced in the mid-to-late game.

---

### **Specialized Equipment Requirement**
- **Concept**: Helpers need specialized tools or workstations to assist effectively.
- **Implementation**:
  - Require "Helper Kits" or upgraded workstations to enable helpers to contribute fully.
  - Without the equipment, helpers provide minimal or no benefit.
- **Effect**: Adds a layer of progression and resource investment to balance the benefits of helpers.

---

### **Time Management and Coordination**
- **Concept**: Helpers need preparation time or downtime between tasks.
- **Implementation**:
  - Introduce a cooldown period for helpers after assisting in a task.
  - Add a short "setup phase" before helpers can begin assisting.
- **Effect**: Encourages players to plan tasks and manage helper workloads more effectively.














Allright i really likeing Helper Skill Dependency and Social Interaction Drawbacks.

Lets start with Helper Skill Dependency, So i think a Crafter is Required to have at least 1 lvl in any contribiting else they will negtivly affect the speed, by a small amount.

in regard to Social Interaction Drawbacks: Add a small negative moodlet if too many pawns are Helping the same pawn. But only for peaple who are Rivals or Ugly. and Add Postive moodlets if they are helping someone who is there Lovers/Fiancés/Fiancées/Spouses/Friends/Beautiful.

Here are some of the moodlits writen out:
"Working with a Rival" or "Its annoying working with 2 Rivals"
"Ew why do i have to work with an ugly person"(The Ugly person wont get this affect)

"I get to see my lover while working"
"Get to see Fiancés/Fiancées"
"working with Spouse"
"Hanging with freand at work"
"Worked with some Beautiful peaple"

Can you make these all about as short as "working with Spouse" or close too


Each Moodlit is its own and one pawn can have multible moodlits from working with peaple. But it goes away as soon as they stop helping.








Hey So i was hopeing to get enlightend, on 2 thing am struggling to track down. 

So am currently Working with Toils_Recipe.DoRecipeWork() and am trying to figure out what class/Methods are responsable for an items being crafted geting assgined a Quality(stuff like Masterwork/Awful ), As well as, at what point dose Food when being crafted Assign weather or not it would have a chance to have food poisoning and putting it on the item. 


To explain my self a little bit my mod is working around the crafting prossess and one of the things i want to add is some additional values to influence the chance's to get particular qualitys, along with setting Limits on what quality can be crafted in particular situations. In that same vain of thought i want to influcnce the chance's of food posioning accureing.

