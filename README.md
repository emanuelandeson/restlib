# RestLib
LibRest simple abstraction to prove rest requests. 

Samples:

#instance
var rest = new RestLib();

#instance with header
var rest = new RestLib(new Dictionary<string, string>
            {
                {"Country","BR"},
                {"City","SP"},
            });

#do post/Patch/Put 
var postStream = await rest.PostStreamAsync("http://localhost:3000", new
{
  name = "name of",
  some = "..."
});
            
#do post/Patch/Put with stream
var postStream = await rest.PostStreamAsync("http://localhost:3000", new
{
  name = "name of",
  some = "..."
});

#do simple get
var resp = await rest.GetAsync<SomeType>("http://localhost:3000");
  
#do get with stream (more perfomance on bigger requests)
var resp = await rest.GetStreamAsync<SomeType>("http://localhost:3000");
