**Helpers for RimWorld**

## **Overview**
Helpers is a quality-of-life mod for RimWorld that's designed to let pawns assist other pawns with various tasks. It's created to improve efficiency, enrich gameplay with skill development, and foster social interactions, making colonies feel more dynamic. The mod avoids creating overpowered situations by introducing trade-offs that maintain challenge and realism.
(Not all of these are implamented yet but is at least part of the plan)
## **Core Features and Mechanics**

### **Helping Mechanic**
   - Adds a new "Help" button to the game interface.
   - Accessible through the right-click context menu when selecting a pawn.
   - Enables one pawn (the helper) to assist another (the worker/Helped) with tasks such as crafting, cooking, and more.
   - Allows multiple helpers to assist a single worker.
   - Contributions are determined by the skill levels of the helpers(useing there Skill for the particular task(Construction, Medical, Cooking, Crafting), and there Helper Skill, Whtich Determins how effetion they are at helping with tasks).
   - helpers Also Contribute 

---

### **Skill Contribution System**

1. **Helper Contribution Formula:**
   - Effective helper contribution is calculated as:
     \[
     \text{Helper Contribution} = (0.5 + \frac{\text{Helper Skill Level}}{40}) \times \text{Helper Work Speed}
     \]
   - For example:
     - A Level 0 helper contributes 50% of their base work speed.
     - A Level 10 helper contributes 75% of their base work speed.
     - A Level 20 helper contributes 100%, significantly speeding up task completion.

2. **Skill Requirements:**
   - Helpers must have at least one level in the relevant skill for the task.
   - Unskilled helpers (skill level <1) slightly reduce work efficiency to simulate "getting in the way."

3. **Experience Gains:**
   - Helpers earn XP in the task's main skill (e.g., Crafting for crafting tasks).
   - Helpers also gain XP in a new "Helping" skill, improving their efficiency when assisting others.

---

### **Trade-Offs and Balancing**

1. **Quality Trade-Offs:**
   - Additional helpers limit the maximum quality of crafted items.
   - Quality penalties scale with the number of helpers, reflecting the “too many cooks” effect.
   - For example:
     - One helper: Max quality capped at “Excellent.”
     - Three helpers: Max quality capped at “Good.”

3. **Mood Dynamics:**
   - Helpers generate moodlets based on their relationships with the worker:
     - Positive moodlets: “Working with Spouse,” “Helping a Friend,” or “Seeing my Lover.”
     - Negative moodlets: “Annoying Rival Nearby” or “Working with Ugly Coworker.”
   - Moodlets disappear when the task ends.

---

## **Future Features and Ideas**

1. **Advanced Mood Integration:**
   - Rivalries or friendships evolve through repeated helper interactions.
   - Helpers who regularly assist others build stronger social bonds.

2. **Settings Customization:**
   - Add new settings for the Helper Contribution Formula, allowing users to modify pawn effectiveness and potentially exceed the default 100% work speed.

3. **Dedicated Helper Assignment:**
   - Enable players to assign pawns as dedicated helpers to others. These helpers prioritize assisting their assigned buddy during work.

4. **Material Transport Assistance:**
   - Helpers can aid the primary worker by transporting materials, improving walking speed, and carrying capacity, or fetching crafting components.

5. **Automatic Rescue and Medical Aid:**
   - Helpers automatically rescue injured allies, bring them to their bed, and begin medical treatment immediately.
   - Outside of critical conditions, helpers with sufficient medical skills will bandage injured allies.

---

### **Task-Specific Details**

**Construction**
- **Furniture Quality:** Mirrors crafting—helpers affect speed, and furniture quality is capped based on the highest skill level present and the helpers' skill.
- **Other Construction Tasks:** Speed is the only factor impacted for structures, floors, repairs, and deconstruction. No quality loss since these don’t have quality metrics.

**Farming & Growing**
- **Tasks Supported:** Sowing seeds, harvesting crops, and cutting plants.
- **Logic:** Helpers reduce time spent on sowing and harvesting but do not alter yield or quality.

**Medical**
- **Healing Tasks:** Bandaging wounds, tending injuries, and performing surgeries.
- **Logic:** Helpers with skill levels below 6 cannot assist with surgeries or advanced treatments. Bandaging requires a minimum level of 3. Higher-level helpers improve success rates and reduce treatment time.

**Animal Handling**
- **Training/Taming:** Helpers with Level 6+ improve training success. Helpers below Level 6 increase failure chances. Speed is not affected.

**Mining**
- **Tasks Supported:** Digging tunnels and extracting resources.
- **Logic:** Helpers reduce mining time, with contributions based on skill level. Lower-skilled helpers have minimal impact on work speed.

---

### **Acknowledgments**
This project heavily benefited from the use of AI tools, including ChatGPT, to assist in various aspects of development. AI tools played a pivotal role in:
- Brainstorming and refining core logic and mechanics.
- Optimizing algorithms and balancing gameplay features.
- Generating and formatting documentation after analyzing my personal notes and the project structure.
- Supporting code refactoring, such as mass replacement during debugging system overhauls.
- Providing grammar and spell-checking to ensure clarity and professionalism.

AI was not just a convenience but an essential tool in the development process. As someone with learning disabilities, AI was invaluable in breaking down complex concepts into digestible pieces, enabling me to learn and apply modding principles in RimWorld for the first time. It also served as a reminder of coding practices I had learned but forgotten over the years since first learning C#.

While AI provided significant assistance, every output was carefully validated to ensure accuracy, relevance, and ethical compliance. Specific examples of AI usage include:
- Commenting code: AI helped document the project by interpreting explanations I provided about methods and classes, ensuring clarity and maintainability.
- Debugging: AI streamlined repetitive tasks like replacing and updating debugging logic, saving time and improving efficiency.
- Code review: AI was a partner in reviewing and refining code, offering suggestions to improve structure and readability.


