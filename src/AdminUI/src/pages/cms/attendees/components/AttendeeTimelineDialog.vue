<template>
  <Dialog
    :visible="true"
    :style="{ width: '90vw', maxWidth: '900px', minWidth: '360px' }"
    :header="`${message.providerCode} | ${item.fullName} - ${item.phoneNumber}`"
    :modal="true"
    :closable="false"
  >
    <Timeline :value="events" align="alternate" class="customized-timeline">
      <template #marker="slotProps">
        <span
          style="width: 2rem; height: 2rem; border-radius: 50%"
          class="flex align-items-center justify-content-center text-white z-10 shadow-sm"
          :style="{ backgroundColor: slotProps.item.color }"
        >
          <i :class="slotProps.item.icon"></i>
        </span>
      </template>
      <template #content="slotProps">
        <Card class="mb-4">
          <template #title>
            {{ slotProps.item.action }}
          </template>
          <template #subtitle>
            <i>{{ $t("AttendeesPage.OccurredAtBy", [formatDate.formatDate(slotProps.item.time), slotProps.item.user]) }}</i>
          </template>
          <template #content>
            <div v-if="slotProps.item.action == 'Sent'" class="flex flex-column gap-2">
              <div v-if="message.responsePayload && message.statusId == MessageStatus.Failed" style="overflow: auto; word-break: break-all">
                <b>Exception:&nbsp;</b><br />
                <code style="font-size: 0.875rem">
                  {{ message.responsePayload }}
                </code>
              </div>
              <div v-if="message.responsePayload && message.statusId != MessageStatus.Failed" style="overflow: auto; word-break: break-all">
                <b>Response:&nbsp;</b><br />
                <code style="font-size: 0.875rem">
                  {{ message.responsePayload }}
                </code>
              </div>
            </div>
            <div v-if="slotProps.item.action == 'Delivered'" class="flex flex-column gap-2">
              <div v-if="message.messageId" style="overflow: auto; word-break: break-all"><b>Message #:&nbsp;</b> {{ message.messageId }}</div>
              <div v-if="message.eventPayload" style="overflow: auto; word-break: break-all">
                <b>Event Payload:&nbsp;</b><br />
                <code style="font-size: 0.875rem">
                  {{ message.eventPayload }}
                </code>
              </div>
            </div>
          </template>
        </Card>
      </template>
    </Timeline>
    <template #footer>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import type { AttendeeItemDto, AttendeeMessageDto } from "@/contracts/Attendees";
import formatDate from "@/plugins/dates/formatDate";
import { MessageStatus } from "@/contracts/Enums";

interface Props {
  item: AttendeeItemDto;
  message: AttendeeMessageDto;
}
const props = defineProps<Props>();

const $t = useI18n().t;
const events = computed(() => {
  const data = [
    {
      time: props.item.createdTime,
      icon: "pi pi-plus",
      color: "#9C27B0",
      action: "Created",
      user: props.item.createdBy,
      content: ""
    },
    {
      time: props.message.createdTime,
      icon: "pi pi-check",
      color: "#673AB7",
      action: "Confirmed",
      user: props.message.createdBy,
      content: ""
    }
  ];
  if (props.message.sentAt) {
    let color = "red";
    try {
      const payload = JSON.parse(props.message.responsePayload!);
      if (payload && (payload.status == "success" || payload.error == 0)) color = "#4CAF50";
    } catch {
      //
    } finally {
      //
    }
    data.push({
      time: props.message.sentAt,
      icon: "pi pi-send",
      color: color,
      action: "Sent",
      user: "System",
      content: ``
    });
  }
  if (props.message.userReceivedAt) {
    data.push({
      time: props.message.userReceivedAt,
      icon: "pi pi-inbox",
      color: "#13ce66",
      action: "Delivered",
      user: "System",
      content: ``
    });
  }
  return data;
});

const emits = defineEmits(["close"]);
</script>

<style lang="scss" scoped>
::v-deep(.customized-timeline) {
  .p-timeline-event:nth-child(even) {
    flex-direction: row;

    .p-timeline-event-content {
      text-align: left;
    }
  }

  .p-timeline-event-opposite {
    flex: 0;
  }
}
</style>
