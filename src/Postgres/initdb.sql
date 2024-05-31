CREATE TABLE notification (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  version INT NOT NULL,
  message VARCHAR(500) NOT NULL,
  email VARCHAR(50) NULL,
  email_notification_status VARCHAR(10) NOT NULL,
  phone VARCHAR(50) NULL,
  phone_notification_status VARCHAR(10) NOT NULL);

CREATE INDEX idx__notification__user_id ON notification(user_id);
