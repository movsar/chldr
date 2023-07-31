### Localization

UI must be translated into three primary languages: Russian, Chechen, English.

Localization is done through .resx files. 
There is a nice Resx Manager extension for VS that makes it easy to translate resource entries.

### Naming conventions for localization entries:
All translation resources, are standard name/value string entries. 

Entries for names of properties, classes and enums should use PascalCase:
- **NumericalCategory**

Everything else must follow Underscore case:
- **Numerical_category**

Entries may, or may not have prefixes, if they do, they're in Pascal Case and are followed by a colon (:)
- **Error:Something_went_wrong**.