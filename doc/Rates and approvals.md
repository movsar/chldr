## Rate

All editable entities Entries, Translations, Sounds along with the user entity, have an integer Rate field, based on which the app determines whether other users can edit / remove that entity.

Range of the integer value determines what you can do with it:

There are five ranges, each associated with a Role:
- MemberRateRange: (1, 10)
- EnthusiastRateRange: (10, 100)
- ContributorRateRange: (100, 1000)
- EditorRateRange: (1000, 10000)
- MaintainerRateRange: (10000, 500000000)

The system determines the Rate Range of the user and the Rate Range of the entity and checks what they can do with it.

### Moderation

All entities added by a Member should be seen only in moderation tab, they should not be available until they're promoted by someone with a higher Role, that is to be seen publicly the entity's Rate must be higher than the higher boundary of MemberRateRange.

This is done to prevent random people adding to the public unintellligeble or offensive content.

### Promotions

Users with higher RR than that of an entity, can promote the entity, effectively changing its Rate value to their own RR's lower boundary. For instance:

User **A** who has Rate **1** (RR = **Member**), adds a new **Entry**. It will get user RR's lower boundary value, i.e. **1**. Now that entry will only be accessible on moderation experience. If a user **B**, who has Rate **15** (RR = **Enthusiast**) logs in they will be able to see that entry with **Promote** and **Remove** buttons. Once they click the Promote button, the system should set Entry's Rate to the lower boundary of the acting user **B**, setting it to **10** in that case. 

After the rate has been changed, they will no longer see the Remove button. The Promote button will be available to detract the operation for **X** hours, after which it too will disappear and only users with still higher RR will be able to see those buttons.

#### Removing an entity

When a user clicks on Remove, it doesn't get deleted immediately, instead its Rate is set to **-1** and it will go to the trash, which will be a special place with negative rate entities.

### User Rate

New users shoould have Rate = **5** (middle of the Member's RR).

Whenever an entity has been promoted, its author should get an increase in their Rate, according to the UserRole of the promoter. If they're a Member it's **1**, Enthusiast - **2** etc.

Whenever an entity has been removed, author should get a decrease by **1** in their Rate value.

A user's Status field must be set to Banned if the user's Rate reaches **0**.