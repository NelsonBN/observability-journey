CREATE TABLE notification (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  version INT NOT NULL,
  body VARCHAR(500) NOT NULL,
  email VARCHAR(50) NULL,
  email_notification_status VARCHAR(10) NOT NULL,
  phone VARCHAR(50) NULL,
  phone_notification_status VARCHAR(10) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL);

CREATE INDEX idx__notification__user_id ON notification(user_id);



CREATE TABLE report_state (
    id INT PRIMARY KEY,
    last_generated_at TIMESTAMPTZ  NOT NULL);
