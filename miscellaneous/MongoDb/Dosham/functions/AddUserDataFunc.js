exports = async function(authEvent) {
  const operationType = authEvent.operationType
  if (operationType !== "CREATE"){
    return;
  }
  
  console.log(EJSON.stringify("trigger run"));
  
  const user = authEvent.user;
  const time = authEvent.time;
  
   // 1. Get a data source client
  const mongodb = context.services.get("mongodb-atlas");
  
  // 2. Get a database & collection
  const db = mongodb.db("dosham-database");
  const collection = db.collection("User");
  
  // 3. Read and write data with MongoDB queries
  
    "RateWeight",
    "Rate",
    "Status",
    "CreatedAt",
    "UpdatedAt"
    
  await collection.insertOne({
    _id: new BSON.ObjectId(user.id),
    Email: user.data.email,
    Rate: 1,
    RateWeight: 1,
    CreatedAt: time,
    UpdatedAt: time,
    Status: 4
  });
};