DROP FUNCTION Enqueue(json);
DROP FUNCTION Dequeue(int, int);
DROP FUNCTION Commit(json);
DROP FUNCTION Clear();

DROP TYPE EnqueueItemType;
DROP TYPE DequeueItemType;
DROP TYPE CommitItemType;

DROP TABLE Queue;
DROP TABLE Setting;

