# my-secrets

If you want to keep your confidential information protected - this is a console UI application to keep your records encrypted.


1. Create .my-secrets file in your home folder

```
{
	"File":"path of local snapshot of data (to work offline)",
	"InitVector":"init vector for AES256 crypto algo. Do not miss it",
	"BlobConnectionString":"If you want to keep every snapshot of your data in azure cloud blobs (encrypted of course) - here is the connection string. If you do not want to - just remove that field from .my-secrets file"
}
```

2. Please make sure that path for localsnapshot exists;
3. Please, use strong password and do not forget InitVector
