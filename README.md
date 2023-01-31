# Chechen Language Dictionary

A project to build an interactive Chechen language dictionary. Started form scratch, aimed for the masses, it's meant to be a platform for those of us who are going to be the takecarers of this ancient language.

## Coding conventions

### Localization expression names


 - Must follow the same style
 - If specified, prefix starts with uppercase letter and ends with a colon (:)
 - Always starts with uppercase letter followed by lowercase letters separated by underscore (_)

Example: **Error:Something_went_wrong**.

# TO DO

- Create a login trigger that inserts custom data record - done
- Pass email if it's not empty when switching from LoginPage to Registration or Password reset pages
- What if the user deletes confirmation email and wants to request a new one?
	(it's possible to call again confirmation func)
- Send activation notification email after email confirmation
- Compress DB after reaching 60Mb
- Add SearchQueryBuilder