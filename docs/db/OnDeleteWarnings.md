### Before Deleting a user, writerId is set to null (client set null) (must include it)
### Before Deleting an admin user, check if he resolved any reports and set ResolverId to null (manually)

### Before Deleting an office, delete suggestions associated with it (manually)

### Before Deleting a service, delete suggestions associated with it (manually)
### Before Deleting a service, delete steps that uses it as an inner service (manually)

### Before deleting a category, cannot delete if it has services|servicesSuggestions because relationship is required
