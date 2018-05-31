using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BadmintonSvc.Models;

namespace BadmintonSvc.Controllers
{
    public class QueueController : ApiController
    {
        private BadmintonSvcContext db = new BadmintonSvcContext();

        [Route("api/Queue/{clubID}")]
        public IQueryable<QueueData> GetQueue(int clubID)
        {
            return db.Queues.Select(q => new QueueData
            {
                ClubID = q.ClubID,
                QueueID = q.QueueID,
                PlayerID = q.PlayerID,
                QStatusID = q.QStatusID,
                SkillsetID = q.SkillsetID,
                PlayerName = q.Player.PlayerName,
                Score = q.Score,
                PlayDateTime = q.PlayDateTime
            }).Where(q => q.QStatusID != 5 && (q.ClubID == clubID) && DbFunctions.TruncateTime(q.PlayDateTime)
                        == DbFunctions.TruncateTime(DateTime.Today.Date)); // add datetime condition
        }

        // GET: api/QueueBySkills
        [Route("api/QueueBySkills/{skillID}")]
        public IQueryable<QueueData> GetQueueBySkills(int skillID, int clubID)
        {
            if (skillID != 0)
            {
                return db.Queues.Select(q => new QueueData
                {
                    ClubID = q.ClubID,
                    QueueID = q.QueueID,
                    PlayerID = q.PlayerID,
                    QStatusID = q.QStatusID,
                    SkillsetID = q.SkillsetID,
                    PlayerName = q.Player.PlayerName,
                    Score = q.Score,
                    PlayDateTime = q.PlayDateTime
                }).Where(q => q.SkillsetID == skillID && (q.QStatusID != 5) &&
                        (q.ClubID == clubID) && DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)); // add datetime condition    
            }// return db.Queues
            else
                return db.Queues.Select(q => new QueueData
                {
                    ClubID = q.ClubID,
                    QueueID = q.QueueID,
                    PlayerID = q.PlayerID,
                    QStatusID = q.QStatusID,
                    SkillsetID = q.SkillsetID,
                    PlayerName = q.Player.PlayerName,
                    Score = q.Score,
                    PlayDateTime = q.PlayDateTime
                }).Where(q => (q.QStatusID != 5) && (q.ClubID == clubID) && DbFunctions.TruncateTime(q.PlayDateTime)
                            == DbFunctions.TruncateTime(DateTime.Today.Date)); // add datetime condition    
        }

        [Route("api/QueuePlaying/{clubID}")]
        public IQueryable<QueueData> GetQueuePlaying(int clubID)
        {
            CheckClubQuantity(clubID);
            return db.QueueTemps.Select(q => new QueueData
            {
                ClubID = q.ClubID,
                QueueID = q.QueueID,
                PlayerID = q.PlayerID,

                QStatusID = q.QStatusID,
                SkillsetID = q.SkillsetID,
                PlayerName = q.Queue.Player.PlayerName,
                QueueOrder = q.QueueOrder,
                QueueTempID = q.QueueTempID,
                PlayDateTime = q.Created
            }).Where(item => item.ClubID == clubID && item.QStatusID == 4 && DbFunctions.TruncateTime(item.PlayDateTime)
                        == DbFunctions.TruncateTime(DateTime.Today.Date)).OrderByDescending(q => q.QueueOrder);
        }

        [Route("api/QueueNext/{clubID}")]
        public IQueryable<QueueData> GetQueueNext(int clubID)
        {
            CheckClubQuantity(clubID);
            return db.QueueTemps.Select(q => new QueueData
            {
                ClubID = q.ClubID,
                QueueID = q.QueueID,
                PlayerID = q.PlayerID,
                QStatusID = q.QStatusID,
                SkillsetID = q.SkillsetID,
                PlayerName = q.Queue.Player.PlayerName,
                QueueOrder = q.QueueOrder,
                QueueTempID = q.QueueTempID,
                PlayDateTime = q.Created
            }).Where(item => item.ClubID == clubID && item.QStatusID == 2 && DbFunctions.TruncateTime(item.PlayDateTime)
                        == DbFunctions.TruncateTime(DateTime.Today.Date)).OrderBy(q => q.QueueOrder);
        }

        public void CheckClubQuantity(int clubID)
        {
            int processCount = db.Processes.Where(p => p.Status != 0).Count();
            if (processCount > 0)
            {
                LogMe("Another process is running!!!", "CheckClubQuantity", "Message");
                return;
            }
            var clubInfo = db.Clubs.Find(clubID);
            try
            {
                LogMe("Verifying Club Occupancy", "CheckClubQuantity", "Message", 0, clubID);
                int courts = clubInfo.NoOfCourts;
                int clubQuantity = (courts * 4); // for next set of players
                int playingCount = db.QueueTemps.Where(q => q.ClubID == clubID && DbFunctions.TruncateTime(q.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).Count();
                int queueCount = db.Queues.Where(q => q.ClubID == clubID && DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date) && q.QStatusID != 5).Count();

                if (clubInfo.Status == 0)
                {
                    clubInfo.Status = 1; //processing algorithm 
                    UpdateClubStatus(clubInfo);

                    if (queueCount >= (clubQuantity + 4))
                    {
                        LogMe("Checking next set of Players...", "CheckClubQuantity", "Message", 0, clubID);
                        MoveNextToPlay(clubID);
                    }

                    if (queueCount > (clubQuantity + 4))
                        clubQuantity = clubQuantity + 4;                   

                    if (playingCount < clubQuantity)
                    {
                        LogMe("Evaualting the queue...", "CheckClubQuantity", "Message", 0, clubID);
                        for (int i = 0; i < courts; i++)
                        {
                            if (EvaluateQ(clubID) < 4)
                                break;
                        }
                        //playingCount = db.QueueTemps.Where(q => q.ClubID == clubID && DbFunctions.TruncateTime(q.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).Count();
                    }
                    clubInfo.Status = 0; //completed algorithm 
                    UpdateClubStatus(clubInfo);
                }
                else
                    LogMe("Algorithm is already running!!!", "CheckClubQuantity", "Message", 0, clubID);
            }
            catch (Exception ex)
            {
                LogMe("Error while verifying the club occupancy: " + ex.ToString(), "CheckClubQuantity", "Error", 0, clubID);
                clubInfo.Status = 0; //completed algorithm 
                UpdateClubStatus(clubInfo);
            }
        }

        // GET: api/QueueMe
        [Route("api/QueueMe/{clubID}")]
        public IQueryable<QueueData> GetQueueMe(int clubID)
        {
            CheckClubQuantity(clubID);
            return db.QueueTemps.Select(q => new QueueData
            {
                ClubID = q.ClubID,
                QueueID = q.QueueID,
                PlayerID = q.PlayerID,
                QStatusID = q.QStatusID,
                SkillsetID = q.SkillsetID,
                PlayerName = q.Queue.Player.PlayerName,
                QueueOrder = q.QueueOrder,
                QueueTempID = q.QueueTempID,
                PlayDateTime = q.Created
            }).Where(item => item.ClubID == clubID && DbFunctions.TruncateTime(item.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)).OrderByDescending(q => q.QueueOrder);
        }

        public int EvaluateQ(int clubID)
        {
            try
            {
                LogMe("Initialising Algorithm ", "QueueMe", "Message", 0, clubID);
                var queueToday = db.Queues.Where(item => DbFunctions.TruncateTime(item.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)
                     && item.ClubID == clubID && (item.QStatusID == 1 || item.QStatusID == 3)).OrderBy(q => q.QueueID);
                int size = 4, otherSkill = 0, skipCount = 0, queueCount = 0;
                int k = 0;
                IList<Queue> curr4, next4, skipped, playersFromNextGrp, playersOtherSkill, playersToAdd;
                Queue playerFromNextGrp, playerOtherSkill;
                queueCount = queueToday.ToList().Count();
                LogMe("Found " + queueCount + " players in the queue", "QueueMe", "Message", 0, clubID);
                if (queueCount >= 4)
                {
                    LogMe("Begin Algorithm ", "QueueMe", "Message", 0, clubID);
                    curr4 = queueToday.Take(size).ToList();
                    k = queueCount - size;
                    if (k >= 4)
                        k = 4;
                    next4 = queueToday.Skip(size).Take(k).ToList(); //verify size value
                    skipped = queueToday.Where(s => s.QStatusID == 3).ToList();
                    skipCount = skipped.Count();

                    LogMe("Skipped count: " + skipCount, "QueueMe", "Message", 0, clubID);

                    switch (skipCount)
                    {
                        case 0: //first time or when no players skipped
                            var skillCount = curr4.GroupBy(s => s.SkillsetID).Select(g => new { SkillID = g.Key, Count = g.Count() });
                            //code hidden 
                        case 1: //one player was skipped
                            int skipSkill = 0;
                            foreach (var sPlayer in skipped)
                                skipSkill = sPlayer.SkillsetID;
                            var playersSameGrp = FindPlayers(curr4, skipSkill, 4);
                        //code hidden 
                        case 2: //2 players were skipped
                            skipSkill = 0;
                            Boolean sameSkill = false; //, adjSkill = false;
                            foreach (var sPlayer in skipped)
                            {
                                if (skipSkill != 0)
                                {
                                    if (sPlayer.SkillsetID == skipSkill)
                                        sameSkill = true;
                                }
                                skipSkill = sPlayer.SkillsetID;
                            }
                            if (sameSkill)
                            {
                                //code hidden 
                            }
                            else //if (adjSkill)
                            {
                                int[] skipArray = { 0, 0 };
                                int i = 0;
                                foreach (var player in skipped)
                                {
                                    skipArray[i] = player.SkillsetID;
                                    i++;
                                }
                                //code hidden 
                            }
                                break;
                        case 3: // 3 players were skipped - less favourable condition
                            var skipGrp = skipped.GroupBy(i => i.SkillsetID).Select(g => new { SkillID = g.Key, Count = g.Count() })
                                     .OrderByDescending(sg => sg.Count).ThenByDescending(sg => sg.SkillID);
                            switch (skipGrp.Count())
                            {
                                //code hidden 
                            }
                    }
                    queueToday = db.Queues.Where(item => DbFunctions.TruncateTime(item.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)
                    && item.ClubID == clubID && (item.QStatusID == 1 || item.QStatusID == 3)).OrderBy(q => q.QueueID);
                    queueCount = queueToday.ToList().Count();
                    LogMe("End of iteration!","EvaluateQ", "Message", clubID);
                }
                else
                    LogMe("Player(s) count is less for starting the algorithm...", "EvaluateQ", "Message", clubID);
                return queueCount;
            }
            catch (Exception ex)
            {
                LogMe("Error found in QueueMe " + ex.ToString(), "QueueMe", "Error");
                throw ex;
            }
        }

        private void ReQueue(IList<Queue> curr4, IList<Queue> next4, int maxSkillID, int nextSkillID, int clubID)
        {
            try
            {
                int otherSkill = 0;
                IList<Queue> playersToAdd;
                LogMe("Begin Re-evaluating the Queue for Skill " + nextSkillID, "ReQueue", "Message");
                var playersFromNextGrp = FindPlayers(next4, nextSkillID, 2);
                if (playersFromNextGrp != null && playersFromNextGrp.Count() != 0)
                {
                    if (playersFromNextGrp.Count() == 2)
                    {
                        playersToAdd = AddToQueue(curr4, nextSkillID);
                        foreach (var player in playersFromNextGrp)
                            playersToAdd.Add(player);
                        if (AddToQueueTemp(playersToAdd, clubID))
                        {
                            LogMe("Added in 134", "EvaluateQ", "Message");
                            MarkSkipped(curr4, maxSkillID);
                        }
                    }
                    else if (playersFromNextGrp.Count() == 1)
                    {
                        playersToAdd = AddToQueue(curr4, nextSkillID);
                        foreach (var player in playersFromNextGrp)
                            playersToAdd.Add(player);
                        otherSkill = FindClosestSkill(curr4, nextSkillID);
                        if (otherSkill != 0)
                        {
                            var playerOtherSkill = FindPlayer(curr4, otherSkill, true);
                            playersToAdd.Add(playerOtherSkill);
                            if (AddToQueueTemp(playersToAdd, clubID))
                            {
                                LogMe("Added in 135", "EvaluateQ", "Message");
                                MarkLeftOverSkipped(curr4);
                            }
                        }
                        else
                        {
                            otherSkill = FindClosestSkill(next4, nextSkillID);
                            var playerOtherSkill = FindPlayer(next4, otherSkill, true);
                            playersToAdd.Add(playerOtherSkill);
                            if (AddToQueueTemp(playersToAdd, clubID))
                            {
                                LogMe("Added in 136", "EvaluateQ", "Message");
                                MarkSkipped(curr4, maxSkillID);
                            }
                        }
                    }
                }
                else
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        if (AddToQueueTemp(curr4, clubID))
                            LogMe("Added in 137", "EvaluateQ", "Message");
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMe("Error re-evaluating the queue with the other two group of players with SkillsetID " + nextSkillID + " " + ex.ToString(), "ReQueue", "Error");
            }
        }

        private void MarkLeftOverSkipped(IList<Queue> group4)
        {
            try
            {
                group4.Where(q => q.QStatusID != 2 && q.QStatusID != 4).ToList()
                        .ForEach(t => t.QStatusID = 3);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void MarkSkipped(IList<Queue> group4, int SkillID)
        {
            try
            {
                group4.Where(q => q.SkillsetID == SkillID).ToList()
                    .ForEach(t => t.QStatusID = 3);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // Skip: api/SkipPlayer/5
        [Route("api/SkipPlayer/{queueID}")]
        [ResponseType(typeof(Queue))]
        public IHttpActionResult PostSkipPlayer(int queueID)
        {
            int skipOrder;
            int clubID, qStatus;
            QueueTemp qt = null;
            Queue queue = null, sQueue = null;
            var process = db.Processes.Where(p => p.ProcessID == 2).FirstOrDefault();
            try
            {
                LogMe("Begin skip player", "PostSkipPlayer", "Message", queueID);
                // hold the process
                process.Status = 1;
                UpdateProcessStatus(process);
                qt = db.QueueTemps.Where(item => item.QueueID == queueID).FirstOrDefault();
                sQueue = db.Queues.Find(queueID);
                if (qt != null)
                {
                    skipOrder = qt.QueueOrder;
                    clubID = qt.ClubID;
                    qStatus = qt.QStatusID;
                    queue = db.Queues.Where(q => q.ClubID == clubID && q.SkillsetID == qt.SkillsetID && (q.QStatusID == 1 || q.QStatusID == 3) &&
                            DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)).FirstOrDefault();
                    //players in the wait queue with the same skill as skipped player
                    if (queue == null)
                    {
                        queue = db.Queues.Where(q => q.ClubID == clubID && (q.QStatusID == 1 || q.QStatusID == 3) &&
                                    DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)).FirstOrDefault();
                    }
                    if (queue != null && sQueue != null)
                    {
                        queue.QStatusID = qStatus;
                        UpdateQueueStatus(queue); // check for erros at this point

                        sQueue.QStatusID = 3;
                        UpdateQueueStatus(queue);

                        QueueTemp st = new QueueTemp();
                        st.QueueID = queue.QueueID;
                        st.ClubID = queue.ClubID;
                        st.PlayerID = queue.PlayerID;
                        st.SkillsetID = queue.SkillsetID;
                        st.QueueOrder = skipOrder;
                        st.QStatusID = qStatus;
                        st.Created = DateTime.Now;
                        db.QueueTemps.Add(st);
                        db.QueueTemps.Remove(qt);
                        db.SaveChanges();

                        LogMe("Player has been skipped", "PostSkipPlayer", "Message", queueID, clubID, qt.PlayerID, qt.QueueTempID);
                    }
                    else
                    {
                        int i = 1;
                        db.QueueTemps.Where(t => t.QueueOrder == skipOrder && t.ClubID == clubID && t.QueueID != queueID &&
                            DbFunctions.TruncateTime(t.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).ToList()
                            .ForEach(t =>
                            {
                                t.QStatusID = 1;
                                i++;
                            });
                        db.SaveChanges();

                        sQueue.QStatusID = 3;
                        UpdateQueueStatus(queue);

                        db.QueueTemps.Remove(qt);
                        db.SaveChanges();

                        if (i == 4)
                        {
                            var courtStatus = db.Courts.Where(c => c.CourtNum == skipOrder && c.ClubID == clubID).FirstOrDefault();
                            if (courtStatus != null)
                            {
                                if (courtStatus.Status == 1)
                                {
                                    courtStatus.Status = 0;
                                    UpdateCourtStatus(courtStatus);
                                }
                            }
                        }
                    }
                }
                else
                    LogMe("Player not found in Queuetemp", "PostSkipPlayer", "Message", queueID);
            }
            catch (Exception ex)
            {
                LogMe("Error skipping the player  " + ex.ToString(), "PostSkipPlayer", "Error", queueID);
            }
            finally
            {
                // release the process
                process.Status = 0;
                UpdateProcessStatus(process);
            }
            return Ok(qt);
        }

        // Swap: api/SwapPlayer/5
        [Route("api/SwapPlayer/{FrQID}/{ToQID}")]
        [ResponseType(typeof(QueueTemp))]
        public IHttpActionResult PostSwapPlayer(int FrQID, int ToQID)
        {
            int FromOrder, ToOrder, FrPlayerID, ToPlayerID, FrSkill, ToSkill;
            QueueTemp FrQT = null, ToQT;
            var process = db.Processes.Where(p => p.ProcessID == 6).FirstOrDefault();
            try
            {
                LogMe("Begin Swap player", "PostSwapPlayer", "Message", FrQID, ToQID);
                // hold the process
                process.Status = 1;
                UpdateProcessStatus(process);

                FrQT = db.QueueTemps.Where(item => item.QueueID == FrQID).First();
                ToQT = db.QueueTemps.Where(item => item.QueueID == ToQID).First();
                if (FrQT != null && ToQT != null)
                {
                    FromOrder = FrQT.QueueOrder;
                    FrPlayerID = FrQT.PlayerID;
                    FrSkill = FrQT.SkillsetID;

                    ToOrder = ToQT.QueueOrder;
                    ToPlayerID = ToQT.PlayerID;
                    ToSkill = ToQT.SkillsetID;

                    FrQT.QueueOrder = ToOrder;
                    FrQT.QueueID = ToQID;
                    FrQT.PlayerID = ToPlayerID;
                    FrQT.SkillsetID = ToSkill;

                    ToQT.QueueOrder = FromOrder;
                    ToQT.QueueID = FrQID;
                    ToQT.PlayerID = FrPlayerID;
                    ToQT.SkillsetID = FrSkill;

                    SwapQueueTemp(FrQT);
                    SwapQueueTemp(ToQT);
                    LogMe("Swapped Queue Order", "PostSwapPlayer", "Message", FrQID, ToQID, FrQT.QueueTempID, ToQT.QueueTempID);
                }
                else
                    LogMe("Player(s) not found in Queuetemp", "PostSwapPlayer", "Message", FrQID, ToQID);
            }
            catch (Exception ex)
            {
                LogMe("Error swapping the player  " + ex.ToString(), "PostSwapPlayer", "Error", FrQID, ToQID);
            }
            finally
            {
                // release the process
                process.Status = 0;
                UpdateProcessStatus(process);
            }

            return Ok(FrQT);
        }

        [Route("api/GameClose")]
        [ResponseType(typeof(IDictionary<String, ClosureData>))]
        public IHttpActionResult PostGameClose(IDictionary<String, ClosureData> closeQ)
        {
            int clubID = 0, playerID, skillID, courtNum = 0, i = 1, score = 0;
            var process = db.Processes.Where(p => p.ProcessID == 1).FirstOrDefault();
            try
            {
                LogMe("Begin Game close", "PostGameClose", "Message");
                if (process.Status == 1)
                {
                    while(process.Status !=0)
                    {
                        System.Threading.Thread.Sleep(1000);
                        process = db.Processes.Where(p => p.ProcessID == 1).FirstOrDefault();
                    }
                    
                    var beginClose = closeQ.ElementAtOrDefault(0);
                    QueueTemp beginTemp = db.QueueTemps.Where(temp => temp.QueueID == beginClose.Value.QueueID).FirstOrDefault();
                    if (beginTemp != null)
                        if (beginTemp.QStatusID == 5)
                            return Ok(beginClose);
                    else
                        return Ok(beginClose);
                }
                // hold the process              
                process.Status = 1;
                UpdateProcessStatus(process);
                Game game = new Game();

                foreach (KeyValuePair<String, ClosureData> q in closeQ)
                {
                    // update queue table
                    Queue queue = db.Queues.Find(q.Value.QueueID);
                    if (queue == null)
                    {
                        LogMe("QueueID does not exist", "PostGameClose", "Error", q.Value.QueueID);
                        process.Status = 0;
                        UpdateProcessStatus(process);
                        return NotFound();
                    }
                    clubID = queue.ClubID;
                    playerID = queue.PlayerID;
                    skillID = queue.SkillsetID;
                    score = q.Value.Score;
                    queue.QStatusID = 5;
                    queue.Score = score;
                    queue.Won = q.Value.Won;
                    CloseQueue(queue);

                    // attributes for game table
                    switch (i)
                    {
                        case 1:
                            game.player1 = playerID;
                            game.ScoreA = score;
                            break;
                        case 2:
                            game.player2 = playerID;
                            break;
                        case 3:
                            game.player3 = playerID;
                            game.ScoreB = score;
                            break;
                        case 4:
                            game.player4 = playerID;
                            break;
                    }
                    i++;

                    var clubInfo = db.Clubs.Find(clubID);
                    //courts = clubInfo.NoOfCourts;

                    var playerInfo = db.Players.Find(playerID);
                    var qstatus = db.QStatus.Find(1);
                    var skillset = db.Skillsets.Find(skillID);

                    var qExists = db.Queues.Where(item => item.ClubID == clubID && item.PlayerID == playerID && item.QStatusID != 5 &&
                                DbFunctions.TruncateTime(item.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)); //add datetime condition
                    if (qExists.Count() != 0)
                    {
                        LogMe("Queue already exists for this player in the club", "PostGameClose", "Message", qExists.First().QueueID, clubID, playerID);
                        process.Status = 0;
                        UpdateProcessStatus(process);
                        return Ok(queue);
                    }

                    // add an entry into queue table
                    Queue Q = new Queue();
                    Q.Club = clubInfo;
                    Q.Player = playerInfo;
                    Q.QStatus = qstatus;
                    Q.Skillset = skillset;
                    Q.Score = 0;
                    db.Queues.Add(Q);
                    db.SaveChanges();

                    LogMe("Game closed; New queue entry added", "PostGameClose", "Message", Q.QueueID, Q.ClubID, Q.PlayerID);

                    // remove entries from queue temp table
                    QueueTemp qt = db.QueueTemps.Where(item => item.QueueID == queue.QueueID).FirstOrDefault();
                    if (qt != null)
                    {
                        courtNum = qt.QueueOrder;
                        game.StartTime = qt.Created;
                        game.EndTime = DateTime.Now;
                        db.QueueTemps.Where(item => item.QueueOrder == courtNum && item.ClubID == clubID &&
                                DbFunctions.TruncateTime(item.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).ToList().ForEach(a => db.QueueTemps.Remove(a));
                        db.SaveChanges();
                        LogMe("Removed from Queue Temp", "PostGameClose", "Message");
                    }
                }

                // add data into game table
                db.Games.Add(game);
                db.SaveChanges();

                // release the next order or the closed court
                var courtStatus = db.Courts.Where(c => c.CourtNum == courtNum && c.ClubID == clubID).FirstOrDefault();
                if (courtStatus != null)
                {
                    if (courtStatus.Status == 1)
                    {
                        courtStatus.Status = 0;
                        UpdateCourtStatus(courtStatus);
                    }
                }                                     
            }
            catch (Exception ex)
            {
                LogMe("Error in Game close " + ex.ToString(), "PostGameClose", "Error", 0, clubID);
            }
            finally
            {
                // release the process
                process.Status = 0;
                UpdateProcessStatus(process);
            }
            CheckClubQuantity(clubID);
            return Ok(closeQ);
        }

        private void MoveNextToPlay(int clubID)
        {
            int courtNum = -1;

            try
            {
                var courtStatus = db.Courts.Where(c => c.ClubID == clubID && c.Status == 0 && c.CourtNum != 0).OrderByDescending(c => c.CourtNum).FirstOrDefault();
                if (courtStatus == null)
                {
                    LogMe("All the courts are occupied!!!", "MoveNextToPlay", "Message");
                    return;
                }
                else
                {
                    courtNum = courtStatus.CourtNum;
                    courtStatus.Status = 1;
                    db.SaveChanges();
                    // move next players to the court
                    var nextInTemp = db.QueueTemps.Where(next => next.ClubID == clubID && next.QueueOrder == 0
                                    && DbFunctions.TruncateTime(next.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).ToList();
                    if (nextInTemp.Count() == 4)
                    {
                        nextInTemp.ForEach(q =>
                        {
                            q.QueueOrder = courtNum;
                            q.QStatusID = 4;
                            db.Queues.Find(q.QueueID).QStatusID = 4;
                        });
                        db.Courts.Where(c => c.ClubID == clubID && c.CourtNum == 0).FirstOrDefault().Status = 0;
                        db.SaveChanges();
                        LogMe("Moved players to court from the queue!!!", "MoveNextToPlay", "Message");
                    }
                    else
                    {
                        courtStatus.Status = 0;
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                LogMe("Error moving next set of players to Court!!!" + ex.InnerException, "MoveNextToPlay", "Error");
                return;
            }
        }

        private void UpdateQueueStatus(Queue q)
        {
            try
            {
                db.Queues.Attach(q);
                db.Entry(q).Property(x => x.QStatusID).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdatePlayerCount(Player p)
        {
            try
            {
                db.Players.Attach(p);
                db.Entry(p).Property(x => x.MixInd).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdateQueueTempStatus(QueueTemp q)
        {
            try
            {
                db.QueueTemps.Attach(q);
                db.Entry(q).Property(x => x.QStatusID).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdateQueueTempOrder(QueueTemp q)
        {
            try
            {
                db.QueueTemps.Attach(q);
                db.Entry(q).Property(x => x.QStatusID).IsModified = true;
                db.Entry(q).Property(x => x.QueueOrder).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void SwapQueueTemp(QueueTemp q)
        {
            try
            {
                db.QueueTemps.Attach(q);
                db.Entry(q).Property(x => x.QueueID).IsModified = true;
                db.Entry(q).Property(x => x.PlayerID).IsModified = true;
                db.Entry(q).Property(x => x.SkillsetID).IsModified = true;
                db.Entry(q).Property(x => x.QueueOrder).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdateClubStatus(Club c)
        {
            try
            {
                db.Clubs.Attach(c);
                db.Entry(c).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdateCourtStatus(Court c)
        {
            try
            {
                db.Courts.Attach(c);
                db.Entry(c).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void UpdateProcessStatus(Process p)
        {
            try
            {
                db.Processes.Attach(p);
                db.Entry(p).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void CloseQueue(Queue q)
        {
            try
            {
                db.Queues.Attach(q);
                db.Entry(q).Property(x => x.QStatusID).IsModified = true;
                db.Entry(q).Property(x => x.Score).IsModified = true;
                db.Entry(q).Property(x => x.Won).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private IList<Queue> AddToQueue(IList<Queue> group4, int SkillID, bool? mixInd = false)
        {
            IList<Queue> addPlayers = null;
            try
            {
                if (mixInd == true)
                {
                    if (SkillID != 0)
                        addPlayers = group4.Where(q => q.SkillsetID == SkillID).OrderBy(q => q.Player.MixInd).ToList();
                    else
                        addPlayers = group4.OrderBy(q => q.Player.MixInd).ToList();
                }
                else
                {
                    if (SkillID != 0)
                        addPlayers = group4.Where(q => q.SkillsetID == SkillID).ToList();
                    else
                        addPlayers = group4.ToList();
                }
                return addPlayers;
            }
            catch (Exception ex)
            {
                LogMe("Error adding Skillset " + SkillID + " to the queue " + ex.ToString(), "AddToQueue", "Error");
                return null;
            }
        }

        private bool AddToQueueTemp(IList<Queue> newPlayers, int clubID)
        {
            int QStatus, courtNum = -1, maxSkill = 0, minSkill = 0, playerSkill = 0, playerID = 0;
            long mixUpCount = 0;
            var process = db.Processes.Where(p => p.ProcessID == 4).FirstOrDefault();
            try
            {
                LogMe("Begin adding a player to Queue", "AddToQueueTemp", "Message");

                if (newPlayers.Distinct().Count() != 4)
                {
                    LogMe("Invalid set of players!!!", "AddToQueueTemp", "Message");
                    return false;
                }

                int processCount = db.Processes.Where(p => p.Status != 0).Count();
                if (processCount > 0)
                {
                    LogMe("Another process is running!!!", "AddToQueueTemp", "Message");
                    return false;
                }

                int courts = db.Clubs.Find(clubID).NoOfCourts;
                int clubQuantity = (courts * 4); // for next set of players
                int playingCount = db.QueueTemps.Where(q => q.ClubID == clubID && DbFunctions.TruncateTime(q.Created) == DbFunctions.TruncateTime(DateTime.Today.Date)).Count();
                int queueCount = db.Queues.Where(q => q.ClubID == clubID && DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date) && q.QStatusID != 5).Count();

                if (playingCount == 0)
                {
                    db.Courts.Where(c => c.ClubID == clubID).ToList().ForEach(c => c.Status = 0);
                    db.SaveChanges();
                }

                var courtStatus = db.Courts.Where(c => c.ClubID == clubID && c.Status == 0).OrderByDescending(c => c.CourtNum).FirstOrDefault();
                if (courtStatus == null)
                {
                    LogMe("All the courts are occupied!!!", "AddToQueueTemp", "Message");
                    return false;
                }
                else
                {
                    courtNum = courtStatus.CourtNum;

                    if (queueCount <= (clubQuantity + 4) && (courtNum == 0))
                    {
                        LogMe("Queue count is less for adding next set of players", "AddToQueueTemp", "Message");
                        return false;
                    }

                    process.Status = 1;
                    UpdateProcessStatus(process);

                    courtStatus.Status = 1;
                    UpdateCourtStatus(courtStatus);

                    if (courtNum == 0)
                        QStatus = 2;
                    else
                        QStatus = 4;
                }

                maxSkill = newPlayers.Max(p => p.SkillsetID); // 1, 2 or 3
                minSkill = newPlayers.Min(p => p.SkillsetID); // 1, 2 or 3

                foreach (Queue player in newPlayers)
                {
                    playerSkill = player.SkillsetID; // 1, 2 or 3
                    playerID = player.PlayerID;

                    if(db.QueueTemps.Where(q=> q.QueueID == player.QueueID).Count() > 0)
                    {
                        process.Status = 0;
                        UpdateProcessStatus(process);

                        courtStatus.Status = 0;
                        UpdateCourtStatus(courtStatus);

                        LogMe("Player set has duplicates!!!", "AddToQueueTemp", "Message");
                        return false;
                    }

                    Queue queue = db.Queues.Find(player.QueueID);
                    queue.QStatusID = QStatus;

                    QueueTemp qt = new QueueTemp();
                    qt.QueueID = player.QueueID;
                    qt.ClubID = clubID;
                    qt.PlayerID = playerID;
                    qt.SkillsetID = playerSkill;
                    qt.QueueOrder = courtNum;
                    qt.QStatusID = QStatus;
                    qt.Created = DateTime.Now;
                    db.QueueTemps.Add(qt);

                    db.SaveChanges();

                    if (minSkill != maxSkill && playerSkill != 1)
                    {
                        if (playerSkill < maxSkill)
                            mixUpCount = -1;
                        else if (playerSkill == maxSkill)
                            mixUpCount = 1;
                        else mixUpCount = 0;

                        Player p = db.Players.Find(playerID);
                        mixUpCount = p.MixInd + mixUpCount;
                        p.MixInd = mixUpCount;
                        UpdatePlayerCount(p);
                    }
                    LogMe("Added player to Queue", "AddToQueue", "Message", player.QueueID, player.ClubID, player.PlayerID, qt.QueueTempID);
                }
            }
            catch (Exception ex)
            {
                LogMe("Error while adding a queue " + ex.ToString(), "AddToQueueTemp", "Error");
                return false;
            }
            finally
            {
                // release the process
                process.Status = 0;
                UpdateProcessStatus(process);
            }
            return true;
        }

        private IList<Queue> AddPlayerToQ(IList<Queue> group4, IList<Queue> playersToAdd, int PlayerCount)
        {
            try
            {
                group4.Where(q => q.QStatusID == 1 && !playersToAdd.Any(p => p.QueueID == q.QueueID))
                      .OrderBy(q => q.Player.MixInd)
                      .Take(PlayerCount).ToList()
                      .ForEach(q => playersToAdd.Add(q));
                return playersToAdd;
            }
            catch (Exception ex)
            {
                LogMe("Error while adding player to Queue " + ex.ToString(), "AddPlayerToQ", "Error");
                return playersToAdd;
            }
        }

        private int FindClosestSkill(IList<Queue> group4, int SkillID)
        {
            int skillSetID = 0;
            try
            {
                var skill = group4.Where(t => (t.SkillsetID == (SkillID + 1)) || (t.SkillsetID == (SkillID - 1))).FirstOrDefault();
                if(skill!=null)
                    skillSetID = skill.SkillsetID;               
            }
            catch (Exception ex)
            {
                LogMe("Error finding closest skill for Skillset: " + SkillID + ex.ToString(), "FindClosestSkill", "Error");
            }
            return skillSetID;
        }

        private Queue FindPlayer(IList<Queue> group4, int SkillID, bool? mixInd = false)
        {
            Queue QPlayer = null;
            try
            {
                if (mixInd == true)
                    QPlayer = group4.Where(q => q.SkillsetID == SkillID).OrderBy(q => q.Player.MixInd).FirstOrDefault();
                else
                    QPlayer = group4.Where(q => q.SkillsetID == SkillID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogMe("Error finding a player with Skillset: " + SkillID + ex.ToString(), "FindPlayer", "Error");
            }
            return QPlayer;
        }

        private IList<Queue> FindPlayers(IList<Queue> group4, int SkillID, int PlayerCount, bool? mixInd = false)
        {
            List<Queue> FoundPlayers = new List<Queue>();
            try
            {
                FoundPlayers = group4.Where(q => q.SkillsetID == SkillID).Take(PlayerCount).ToList();
                if (mixInd == true)
                    FoundPlayers = group4.Where(q => q.SkillsetID == SkillID).OrderBy(q => q.Player.MixInd).Take(PlayerCount).ToList();
            }
            catch (Exception ex)
            {
                LogMe("Error finding players " + PlayerCount + "  with Skillset: " + SkillID + ex.ToString(), "FindPlayers", "Error");
            }
            return FoundPlayers;
        }

        private Queue FindPlayerNotSkipped(IList<Queue> group4, int SkillID)
        {
            Queue QPlayer = null;
            try
            {
                QPlayer = group4.Where(q => q.SkillsetID == SkillID && q.QStatusID != 3).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogMe("Error finding a player not skipped with Skillset: " + SkillID + ex.ToString(), "FindPlayerNotSkipped", "Error");
            }
            return QPlayer;
        }

        private IList<Queue> FindPlayersNotSkipped(IList<Queue> group4, int SkillID, int PlayerCount, bool? mixInd = false)
        {
            List<Queue> FoundPlayers = null;
            try
            {
                FoundPlayers = group4.Where(q => q.SkillsetID == SkillID && q.QStatusID != 3).Take(PlayerCount).ToList();
                if (mixInd == true)
                    FoundPlayers = group4.Where(q => q.SkillsetID == SkillID && q.QStatusID != 3).OrderBy(q => q.Player.MixInd).Take(PlayerCount).ToList();
            }
            catch (Exception ex)
            {
                LogMe("Error finding players " + PlayerCount + "  not skipped with Skillset: " + SkillID + ex.ToString(), "FindPlayersNotSkipped", "Error");
            }
            return FoundPlayers;
        }

        // PUT: api/Queue/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQueue(int id, Queue queue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != queue.QueueID)
            {
                return BadRequest();
            }

            db.Entry(queue).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Queue
        [Route("api/Queue")]
        [ResponseType(typeof(Queue))]
        public IHttpActionResult PostQueue([FromBody]Queue queue)
        {
            try
            {
                LogMe("Begin Post Queue", "PostQueue", "Message", 0, queue.ClubID, queue.PlayerID);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var qExists = db.Queues.Where(item => item.ClubID == queue.ClubID && item.PlayerID == queue.PlayerID && item.QStatusID != 5 &&
                                DbFunctions.TruncateTime(item.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)); //add datetime condition
                if (qExists.Count() != 0)
                {
                    LogMe("Queue already exists for this player in the club", "PostQueue", "Message", qExists.First().QueueID, queue.ClubID, queue.PlayerID);
                    return Ok(queue);
                }
                var clubInfo = db.Clubs.Find(queue.ClubID);
                var playerInfo = db.Players.Find(queue.PlayerID);
                var qstatus = db.QStatus.Find(queue.QStatusID);

                Skillset skillset = null;
                if (clubInfo.SkillPredefined)
                {
                    if(playerInfo.SkillsetID != 0)
                        skillset = db.Skillsets.Find(playerInfo.SkillsetID);
                }
                if(skillset == null)
                    skillset = db.Skillsets.Find(queue.SkillsetID);

                Queue Q = new Queue();
                Q.Club = clubInfo;
                Q.Player = playerInfo;
                Q.QStatus = qstatus;
                Q.Skillset = skillset;
                Q.Score = queue.Score;
                Q.PlayDateTime = queue.PlayDateTime;
                db.Queues.Add(queue);
                db.SaveChanges();
                LogMe("Queue Created", "PostQueue", "Message", Q.QueueID, Q.ClubID, Q.PlayerID);
                return Ok(Q);
            }
            catch (Exception ex)
            {
                LogMe("Error while adding a queue: " + ex.ToString(), "PostQueue", "Error", queue.QueueID, queue.ClubID, queue.PlayerID);
                return NotFound();
            }
        }

        // DELETE: api/Queue/5
        [Route("api/DeleteQueue/{queueID}")]
        [ResponseType(typeof(Queue))]
        public IHttpActionResult PostDeleteQueue(int queueID)
        {
            var process = db.Processes.Where(p => p.ProcessID == 3).FirstOrDefault();
            try
            {
                LogMe("Begin Queue Delete", "DeleteQueue", "Message", queueID);
                // hold the process
                process.Status = 1;
                UpdateProcessStatus(process);

                int clubID, deleteOrder, skillID, qStatus;
                Queue queue;
                var deleteTemp = db.QueueTemps.Where(item => item.QueueID == queueID).FirstOrDefault();
                if (deleteTemp != null)
                {
                    clubID = deleteTemp.ClubID;
                    deleteOrder = deleteTemp.QueueOrder;
                    skillID = deleteTemp.SkillsetID;
                    qStatus = deleteTemp.QStatusID;

                    queue = db.Queues.Where(q => q.ClubID == clubID && q.SkillsetID == skillID && (q.QStatusID == 1 || q.QStatusID == 3) && 
                            DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)).FirstOrDefault();
                    //players in the wait queue with the same skill as deleted player
                    if (queue == null)
                    {
                        queue = db.Queues.Where(q => q.ClubID == clubID && (q.QStatusID == 1 || q.QStatusID == 3) && // any players in wait queue
                                    DbFunctions.TruncateTime(q.PlayDateTime) == DbFunctions.TruncateTime(DateTime.Today.Date)).FirstOrDefault();
                    }
                    if (queue != null)
                    {
                        queue.QStatusID = qStatus;
                        UpdateQueueStatus(queue); // check for erros at this point

                        QueueTemp st = new QueueTemp();
                        st.QueueID = queue.QueueID;
                        st.ClubID = queue.ClubID;
                        st.PlayerID = queue.PlayerID;
                        st.SkillsetID = queue.SkillsetID;
                        st.QueueOrder = deleteOrder;
                        st.QStatusID = qStatus;
                        st.Created = DateTime.Now;
                        db.QueueTemps.Add(st);
                        //db.QueueTemps.Remove(deleteTemp);
                        db.SaveChanges();
                    }
                    else
                    {
                        int i = 1;
                        db.QueueTemps.Where(t => t.QueueOrder == deleteOrder && t.ClubID == clubID && t.QueueID != queueID &&
                                            DbFunctions.TruncateTime(t.Created) == DbFunctions.TruncateTime(DateTime.Today.Date))
                                            .ToList().ForEach(t =>
                                            {
                                                db.QueueTemps.Remove(t);
                                                i++;
                                                db.Queues.Find(t.QueueID).QStatusID = 1;
                                            });
                        db.SaveChanges();

                        if (i == 4)
                        {
                            var courtStatus = db.Courts.Where(c => c.CourtNum == deleteOrder && c.ClubID == clubID).FirstOrDefault();
                            if (courtStatus != null)
                            {
                                if (courtStatus.Status == 1)
                                {
                                    courtStatus.Status = 0;
                                    UpdateCourtStatus(courtStatus);
                                }
                            }
                        }
                    }
                }
                queue = db.Queues.Find(queueID);
                if (queue != null)
                {
                    db.Queues.Remove(queue);
                    db.SaveChanges();
                }

                LogMe("Player has been deleted", "DeleteQueue", "Message", queueID);
                return Ok(queue);
            }
            catch (Exception ex)
            {
                LogMe("Error while deleting the queue: " + ex.ToString(), "DeleteQueue", "Error", queueID);
                return NotFound();
            }
            finally
            {
                // release the process
                process.Status = 0;
                UpdateProcessStatus(process);
            }
        }

        // DELETE: api/Queue/5
        [Route("api/PostLogger")]
        [ResponseType(typeof(Logger))]
        public IHttpActionResult PostLogger(Logger log)
        {
            try
            {
                LogMe("Begin Post Logger", "PostLogger", "Message", log.QueueID, log.ClubID, log.PlayerID, log.QueueTempID);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (log != null)
                {
                    LogMe(log.LogMessage, log.LogSource, log.LogType, log.QueueID, log.ClubID, log.PlayerID, log.QueueTempID);
                    LogMe("Log Information added successfully!!!", "PostLogger", "Message", log.QueueID, log.ClubID, log.PlayerID, log.QueueTempID);
                }
                LogMe("End Post Logger", "PostLogger", "Message", log.QueueID, log.ClubID, log.PlayerID, log.QueueTempID);
            }
            catch(Exception ex)
            {
                LogMe(ex.Message, "PostLogger", "Error", log.QueueID, log.ClubID, log.PlayerID, log.QueueTempID);
            }
            return Ok(log);
        }

        private void LogMe(String strMessage, String strSource, String strType, int? QueueID = 0, int? ClubID = 0, int? PlayerID = 0, int? TempID = 0)
        {
            Logger log = new Logger();
            try
            {
                log.ClubID = ClubID;
                log.PlayerID = PlayerID;
                log.QueueID = QueueID;
                log.QueueTempID = TempID;
                log.LogMessage = strMessage;
                log.LogSource = strSource;
                log.LogType = strType;
                db.Logs.Add(log);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.ClubID = ClubID;
                log.PlayerID = PlayerID;
                log.QueueID = QueueID;
                log.QueueTempID = TempID;
                log.LogMessage = ex.ToString();
                log.LogSource = "LogMe";
                log.LogType = "Error";
                db.Logs.Add(log);
                db.SaveChanges();
            }
            finally
            {
                log = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QueueExists(int id)
        {
            return db.Queues.Count(e => e.QueueID == id) > 0;
        }
    }
}