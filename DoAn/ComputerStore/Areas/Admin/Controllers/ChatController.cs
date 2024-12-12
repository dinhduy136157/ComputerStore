using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        private readonly ComputerStoreEntities _context;

        public ChatController()
        {
            _context = new ComputerStoreEntities();
        }

        // Action để bắt đầu chat
        public ActionResult StartChat(int userId, int adminId)
        {
            var chatSession = new ChatSession
            {
                UserID = userId,
                AdminID = adminId,
                StartTime = DateTime.Now,
            };

            _context.ChatSessions.Add(chatSession);
            _context.SaveChanges();

            return RedirectToAction("ChatWindowUser", new { sessionId = chatSession.SessionID });
        }

        // Hiển thị cửa sổ chat cho người dùng
        public ActionResult ChatWindowUser(int sessionId)
        {
            var messages = _context.ChatMessages.Where(m => m.SessionID == sessionId).ToList();
            ViewBag.Messages = messages;
            ViewBag.SessionID = sessionId;

            return View("ChatWindowUser");
        }

        // Hiển thị cửa sổ chat cho admin
        public ActionResult ChatWindowAdmin(int sessionId)
        {
            var messages = _context.ChatMessages.Where(m => m.SessionID == sessionId).ToList();
            ViewBag.Messages = messages;
            ViewBag.SessionID = sessionId;

            return View("ChatWindowAdmin");
        }

        // Action gửi tin nhắn
        [HttpPost]
        public ActionResult SendMessage(int sessionId, int senderId, string messageText)
        {
            var message = new ChatMessage
            {
                SessionID = sessionId,
                SenderID = senderId,
                MessageText = messageText,
                Timestamp = DateTime.Now,
            };

            _context.ChatMessages.Add(message);
            _context.SaveChanges();

            // Tự động trả lời admin nếu người dùng gửi tin
            if (senderId != _context.ChatSessions.FirstOrDefault(s => s.SessionID == sessionId)?.AdminID)
            {
                var adminMessage = new ChatMessage
                {
                    SessionID = sessionId,
                    SenderID = _context.ChatSessions.FirstOrDefault(s => s.SessionID == sessionId)?.AdminID,
                    MessageText = "Cảm ơn bạn đã liên hệ. Admin sẽ trả lời sớm.",
                    Timestamp = DateTime.Now,
                };
                _context.ChatMessages.Add(adminMessage);
                _context.SaveChanges();
            }

            return RedirectToAction(senderId == _context.ChatSessions.FirstOrDefault(s => s.SessionID == sessionId)?.AdminID ? "ChatWindowAdmin" : "ChatWindowUser", new { sessionId = sessionId });
        }
    }

}
