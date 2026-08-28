
db = db.getSiblingDB("notificationdb");

db.createCollection("notificationJobs");

db.notificationJobs.createIndex(
    { messageId: 1 },
    { unique: true, name: "ux_notificationJobs_messageId" }
);

print("init-mongo.js: coleccion e indice de NotificationService creados en 'notificationdb'.");
